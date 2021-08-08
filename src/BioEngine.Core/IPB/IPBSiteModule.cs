namespace BioEngine.Core.IPB
{
    public class IPBSiteModule : IPBModule<IpbSiteModuleOptions>
    {
        public override string OptionsKey => "Ipb:Site";
    }

    public class IpbSiteModuleOptions : IPBModuleOptions
    {
    }
}
