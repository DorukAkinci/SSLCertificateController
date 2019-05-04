using System.Configuration;

namespace SSLCertificateController
{
    class Initializations : ConfigurationSection
    {
        /// </summary>
        public const string SectionName = "Initializations";

        private const string EndpointCollectionName = "Websites";

        [ConfigurationProperty(EndpointCollectionName)]
        [ConfigurationCollection(typeof(WebsitesCollection), AddItemName = "add")]
        public WebsitesCollection ConnectionManagerEndpoints { get { return (WebsitesCollection)base[EndpointCollectionName]; } }
    }

    class WebsitesCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new WebsiteElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WebsiteElement)element).Address + "_" + ((WebsiteElement)element).Type;
        }
    }

    class WebsiteElement : ConfigurationElement
    {
        [ConfigurationProperty("Address", IsRequired = true)]
        public string Address
        {
            get
            {
                return (string)this["Address"];
            }
            set
            {
                this["Address"] = value;
            }
        }


        [ConfigurationProperty("Type", IsRequired = true)]
        public string Type
        {
            get
            {
                return (string)this["Type"];
            }
            set
            {
                this["Type"] = value;
            }
        }

        [ConfigurationProperty("Thumbprint", IsRequired = true)]
        public string Thumbprint
        {
            get
            {
                return (string)this["Thumbprint"];
            }
            set
            {
                this["Thumbprint"] = value;
            }
        }
    }
}