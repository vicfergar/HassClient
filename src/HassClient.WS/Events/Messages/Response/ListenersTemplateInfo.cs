namespace HassClient.WS.Messages
{
    internal class ListenersTemplateInfo
    {
        public bool All { get; set; }

        public string[] Entities { get; set; }

        public string[] Domains { get; set; }

        public bool Time { get; set; }
    }
}
