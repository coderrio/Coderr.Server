namespace codeRR.Server.Infrastructure.Plugins
{
    /// <summary>
    ///     Main menu
    /// </summary>
    public class MenuConfiguration
    {
        private readonly MenuItem _menu = new MenuItem("GlobalOverview", "Overview", "#/");

        public MenuConfiguration()
        {
            _menu.Add("Application", "Application", "#/application/:applicationId/");
            _menu.Add("Incident", "Incident", "#/application/:applicationId/incident/:incidentId/");
            _menu.Add("System", "System", "#/system");
        }

        /// <summary>
        ///     Add to system menu (where the 'settings' and 'admin panel' menu items are visible)
        /// </summary>
        /// <param name="name">
        ///     menu identifier (assigned as HTML Element id), so try to use a somewhat unique name, or we'll get a
        ///     very sad face when the menu stop working.
        /// </param>
        /// <param name="title">Menu item title</param>
        /// <param name="spaRoute">Route in SPA. Do note that the same route will be used when requesting information for the page.</param>
        public void AddToSystemMenu(string name, string title, string spaRoute)
        {
            _menu["System"].Add(name, title, spaRoute);
        }

        /// <summary>
        ///     Generate menu
        /// </summary>
        /// <returns></returns>
        public MenuItem BuildMenu()
        {
            return _menu;
        }
    }
}