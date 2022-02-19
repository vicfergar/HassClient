"""Script to create LLAT for ludeeus/setup-homeassistant action."""
import asyncio
from datetime import timedelta
import logging
import random
import string

from homeassistant import runner
from homeassistant.auth import auth_manager_from_config
from homeassistant.auth.models import TOKEN_TYPE_LONG_LIVED_ACCESS_TOKEN
from homeassistant.core import HomeAssistant

CONFIG_DIR = "/config"


async def create_token():
    """Create token."""
    logging.getLogger("homeassistant").setLevel(logging.CRITICAL)
    hass = HomeAssistant()
    hass.config.config_dir = CONFIG_DIR
    hass.auth = await auth_manager_from_config(hass, [{"type": "homeassistant"}], [])
    hass.auth._store._set_defaults()
    provider = hass.auth.auth_providers[0]
    await provider.async_initialize()

    ## Add user
    username = "".join(random.choice(string.ascii_lowercase) for i in range(32))
    password = "".join(random.choice(string.ascii_lowercase) for i in range(32))
    provider.data.add_auth(username, password)
    user = await hass.auth.async_create_user(username, group_ids=["system-admin"])
    await hass.auth.async_update_user(user, is_active=True)

    ## Create token
    token = await hass.auth.async_create_refresh_token(
        user,
        token_type=TOKEN_TYPE_LONG_LIVED_ACCESS_TOKEN,
        client_name="action-runner",
        access_token_expiration=timedelta(hours=4),
    )

    ## Store the data
    await provider.data.async_save()
    await hass.auth._store._store.async_save(hass.auth._store._data_to_save())

    print(hass.auth.async_create_access_token(token))

    await hass.async_stop()


asyncio.set_event_loop_policy(runner.HassEventLoopPolicy(False))
asyncio.run(create_token())
