﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using System.Web.WebPages;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.DataSources;
using ToSic.Eav.Interfaces;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent.Adam;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Edit.InPageEditingSystem;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Interfaces;
using ToSic.SexyContent.Razor.Helpers;
using ToSic.SexyContent.Search;

namespace ToSic.SexyContent.Razor
{
    /// <summary>
    /// The core page type for delivering a 2sxc page
    /// Provides context infos like the Dnn object, helpers like Edit and much more. 
    /// </summary>
    public abstract class SexyContentWebPage : WebPageBase, IAppAndDataHelpers
    {
        #region Helpers

        protected internal HtmlHelper Html { get; internal set; }

        protected internal UrlHelper Url { get; internal set; }

        protected internal ILinkHelper Link => AppAndDataHelpers.Link;

        // <2sic>
        protected internal SxcInstance Sexy { get; set; }
        protected internal AppAndDataHelpers AppAndDataHelpers { get; set; }
        // </2sic>

        #endregion

        #region BaseClass Overrides

        protected override void ConfigurePage(WebPageBase parentPage)
        {
            base.ConfigurePage(parentPage);

            // Child pages need to get their context from the Parent
            Context = parentPage.Context;

            // Return if parent page is not a SexyContentWebPage

            // 2016-05-02 if (!(parentPage is SexyContentWebPage)) return;    // 2016-02-22 believe this is necessary with dnn 8 because this razor uses a more complex inheritance with Type<T>
            //if (parentPage.GetType().BaseType != typeof(SexyContentWebPage)) return;

            var typedParent = parentPage as SexyContentWebPage;
            if (typedParent == null) return;

            Html = typedParent.Html;
            Url = typedParent.Url;
            Sexy = typedParent.Sexy;
            AppAndDataHelpers = typedParent.AppAndDataHelpers;
        }

        #endregion


        #region AppAndDataHelpers implementation

        /// <inheritdoc />
        public DnnHelper Dnn => AppAndDataHelpers.Dnn;

        /// <inheritdoc />
        public SxcHelper Sxc => AppAndDataHelpers.Sxc;

        /// <inheritdoc />
        public new App App => AppAndDataHelpers.App;

        /// <inheritdoc />
        public ViewDataSource Data => AppAndDataHelpers.Data;

        public IPermissions Permissions => Sexy.Environment.Permissions;

        #region AsDynamic in many variations
        /// <inheritdoc />
        public dynamic AsDynamic(IEntity entity) => AppAndDataHelpers.AsDynamic(entity);
        

        /// <inheritdoc />
        public dynamic AsDynamic(dynamic dynamicEntity) =>  AppAndDataHelpers.AsDynamic(dynamicEntity);


        /// <inheritdoc />
        public dynamic AsDynamic(KeyValuePair<int, IEntity> entityKeyValuePair) =>  AppAndDataHelpers.AsDynamic(entityKeyValuePair.Value);


        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IDataStream stream) => AppAndDataHelpers.AsDynamic(stream.List);


        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IDictionary<int, IEntity> list) =>  AppAndDataHelpers.AsDynamic(list);

        /// <inheritdoc />
        public IEntity AsEntity(dynamic dynamicEntity) => AppAndDataHelpers.AsEntity(dynamicEntity);


        /// <inheritdoc />
        public IEnumerable<dynamic> AsDynamic(IEnumerable<IEntity> entities) => AppAndDataHelpers.AsDynamic(entities);

        #endregion

        #region Data Source Stuff
        public IDataSource CreateSource(string typeName = "", IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
        {
            return AppAndDataHelpers.CreateSource(typeName, inSource, configurationProvider);
        }

        public T CreateSource<T>(IDataSource inSource = null, IValueCollectionProvider configurationProvider = null)
        {
            return AppAndDataHelpers.CreateSource<T>(inSource, configurationProvider);
        }

		/// <summary>
		/// Create a source with initial stream to attach...
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="inStream"></param>
		/// <returns></returns>
		public T CreateSource<T>(IDataStream inStream) =>  AppAndDataHelpers.CreateSource<T>(inStream);

        #endregion

        #region Content, Presentation, ListContent, ListPresentation and List
        public dynamic Content => AppAndDataHelpers.Content;

        public dynamic Presentation => AppAndDataHelpers.Content?.Presentation;

        public dynamic ListContent => AppAndDataHelpers.ListContent;

        public dynamic ListPresentation => AppAndDataHelpers.ListContent?.ListPresentation;

        [Obsolete("This is an old way used to loop things - shouldn't be used any more - will be removed in a future version")]
        public List<Element> List => AppAndDataHelpers.List;
        #endregion

        #endregion


        /// <summary>
        /// Creates instances of the shared pages with the given relative path
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public dynamic CreateInstance(string relativePath)
        {
            var path = NormalizePath(relativePath);

            if(!File.Exists(HostingEnvironment.MapPath(path)))
                throw new FileNotFoundException("The shared file does not exist.", path);

            var webPage = (SexyContentWebPage)CreateInstanceFromVirtualPath(path);
            webPage.ConfigurePage(this);
            return webPage;
        }


        /// <summary>
        /// Override this to have your code change the (already initialized) Data object. 
        /// If you don't override this, nothing will be changed/customized. 
        /// </summary>
        public virtual void CustomizeData()
        {
        }

        public virtual void CustomizeSearch(Dictionary<string, List<ISearchInfo>> searchInfos, ModuleInfo moduleInfo, DateTime beginDate)
        {
        }

        public InstancePurposes InstancePurpose { get; set; }


        #region Adam 

        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        public AdamNavigator AsAdam(DynamicEntity entity, string fieldName) =>  AppAndDataHelpers.AsAdam(entity, fieldName);
        

        /// <summary>
        /// Provides an Adam instance for this item and field
        /// </summary>
        /// <param name="entity">The entity, often Content or similar</param>
        /// <param name="fieldName">The field name, like "Gallery" or "Pics"</param>
        /// <returns>An Adam object for navigating the assets</returns>
        public AdamNavigator AsAdam(IEntity entity, string fieldName) =>  AppAndDataHelpers.AsAdam(entity, fieldName);

        #endregion

        #region Edit

        /// <summary>
        /// Helper commands to enable in-page editing functionality
        /// Use it to check if edit is enabled, generate context-json infos and provide toolbar buttons
        /// </summary>
        public IInPageEditingSystem Edit => AppAndDataHelpers.Edit;

        #endregion
    }


}