/*
' Copyright (c) 2014  Christoc.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Web.UI.WebControls;
using Christoc.Modules.CoursePluggComment.Components;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using Plugghest.Base2;
using Plugghest.Modules.PlugghestControls;

namespace Christoc.Modules.CoursePluggComment
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from CoursePluggCommentModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : CoursePluggCommentModuleBase, IActionable
    {
        public string CultureCode;
        public int CoursePluggId;
        public CoursePluggEntity currentCPE;
        public int CourseId;
        public CourseContainer cc;
        public int PluggId;
        public bool InCreationLanguage;
        public bool IsAuthorized;
        public int Edit;
        public int Translate;
        public int Remove;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                BaseHandler bh = new BaseHandler();
                CultureCode = (Page as DotNetNuke.Framework.PageBase).PageCulture.Name;

                string coursePluggIdStr = Page.Request.QueryString["cp"];
                if (coursePluggIdStr == null)    //This is a Plugg outside a course: no menu
                    return;

                bool isNum = int.TryParse(coursePluggIdStr, out CoursePluggId);
                if (!isNum)
                    return;

                currentCPE = bh.GetCPEntity(CoursePluggId);
                if (currentCPE == null)
                    return;
                CourseId = currentCPE.CourseId;
                cc = new CourseContainer(CultureCode, CourseId);
                if (cc == null)
                    return;

                InCreationLanguage = (cc.TheCourse.CreatedInCultureCode == CultureCode);
                IsAuthorized = ((this.UserId != -1 && cc.TheCourse.WhoCanEdit == EWhoCanEdit.Anyone) || cc.TheCourse.CreatedByUserId == this.UserId || (UserInfo.IsInRole("Administator")));
                Edit = !string.IsNullOrEmpty(Page.Request.QueryString["editcp"]) ? Convert.ToInt16(Page.Request.QueryString["editcp"]) : -1;
                Translate = !string.IsNullOrEmpty(Page.Request.QueryString["translatecp"]) ? Convert.ToInt16(Page.Request.QueryString["translatecp"]) : -1;

                PHText theComment = bh.GetCurrentVersionText(CultureCode, CoursePluggId, ETextItemType.CoursePluggText);
                if (theComment != null && Edit == -1 && Translate == -1)
                    TheText.Text = theComment.Text;

                #region hide/display controls
                if (!InCreationLanguage && UserId > -1 && Translate == -1)
                {
                    phTranslate.Visible = true;
                    hlTranslate.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, "", "translatecp=0", "cp=" + CoursePluggId);
                }

                if (InCreationLanguage && IsAuthorized && Edit == -1)
                {
                    phEditCPComment.Visible = true;
                    if (theComment==null)
                        hlEditCPComment.Text = Localization.GetString("AddCPComment", this.LocalResourceFile);
                    else
                        hlEditCPComment.Text = Localization.GetString("EditCPComment", this.LocalResourceFile);
                    hlEditCPComment.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, "", "editcp=0", "cp=" + CoursePluggId);
                }

                if (!InCreationLanguage && UserId > -1 && Translate > -1)
                {
                    phExitTranslate.Visible = true;
                    hlExitTranslate.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(TabId, "", "cp=" + CoursePluggId);
                    RichControl ucR = (RichControl)this.LoadControl("/DesktopModules/PlugghestControls/RichControl.ascx");
                    if ((ucR != null))
                    {
                        ucR.ModuleConfiguration = this.ModuleConfiguration;
                        ucR.ItemId = CoursePluggId;
                        ucR.CultureCode = CultureCode;
                        ucR.CreatedInCultureCode = CultureCode;
                        ucR.CreatedInCultureCode = cc.TheCourse.CreatedInCultureCode;
                        ucR.ControlOrder = 1;
                        ucR.ItemType = ETextItemType.CoursePluggText;
                        ucR.Case = EControlCase.ViewAllowTranslate;
                        if (Translate == 1)
                            ucR.Case = EControlCase.Translate;
                        ucR.LocalResourceFile = "/DesktopModules/PlugghestControls/App_LocalResources/RichControl.ascx";
                        phTheText.Controls.Add(ucR);
                    }
                }

                if (InCreationLanguage && IsAuthorized && Edit > -1)
                {
                    plInfo.Visible = true;
                    RichControl ucR = (RichControl)this.LoadControl("/DesktopModules/PlugghestControls/RichControl.ascx");
                    if ((ucR != null))
                    {
                        ucR.ModuleConfiguration = this.ModuleConfiguration;
                        ucR.ItemId = CoursePluggId;
                        ucR.CultureCode = CultureCode;
                        ucR.CreatedInCultureCode = CultureCode;
                        ucR.ControlOrder = 1;
                        ucR.ItemType = ETextItemType.CoursePluggText;
                        ucR.Case = EControlCase.Edit;
                        ucR.LocalResourceFile = "/DesktopModules/PlugghestControls/App_LocalResources/RichControl.ascx";
                        phTheText.Controls.Add(ucR);
                    }
                }
                #endregion

                

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection
                    {
                        {
                            GetNextActionID(), Localization.GetString("EditModule", LocalResourceFile), "", "", "",
                            EditUrl(), false, SecurityAccessLevel.Edit, true, false
                        }
                    };
                return actions;
            }
        }
    }
}