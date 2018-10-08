using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.Form.Core.Configuration;
using Sitecore.Publishing;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sitecore.Support.Form.Core
{
    public class PublishItemExtension
    {
        private const string GuidPattern = @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}";

        public void PublishFormChildItems(object sender, EventArgs args)
        {
            string str = IDs.FormInterpreterID.ToString();
            string str2 = IDs.FormMvcInterpreterID.ToString();
            SitecoreEventArgs args2 = args as SitecoreEventArgs;
            Publisher publisher = args2.Parameters.FirstOrDefault<object>() as Publisher;
            if (publisher.Options.PublishRelatedItems && (publisher.Options.Mode == PublishMode.SingleItem))
            {
                string input = publisher.Options.RootItem.Fields["__Final renderings"].Value;
                if (input.Contains(str) || input.Contains(str2))
                {
                    Database sourceDatabase = publisher.Options.SourceDatabase;
                    foreach (object obj2 in Regex.Matches(input, @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}"))
                    {
                        Item item = sourceDatabase.GetItem(obj2.ToString());
                        if ((item != null) && (item.TemplateID == IDs.FormTemplateID))
                        {
                            Database database = Database.GetDatabase("web");
                            this.PublishItem(item, sourceDatabase, database, PublishMode.SingleItem);
                        }
                    }
                }
            }
        }
        private void PublishItem(Item item, Database sourceDB, Database targetDB, PublishMode mode)
        {
            PublishOptions options = new PublishOptions(sourceDB, targetDB, mode, item.Language, DateTime.Now)
            {
                RootItem = item,
                Deep = true
            };
            new Publisher(options).Publish();
        }
    }
}