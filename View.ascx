<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="Christoc.Modules.CoursePluggComment.View" %>

<asp:PlaceHolder ID="phEditCPComment" runat="server" Visible ="false">
    <asp:HyperLink ID="hlEditCPComment" style="font-size: xx-small; float: right;" runat="server" /><br />
</asp:PlaceHolder>

<asp:PlaceHolder ID="phTranslate" runat="server" Visible ="false">
    <asp:HyperLink ID="hlTranslate" style="font-size: xx-small; float: right;" runat="server" resourcekey="Translate" /><br />
</asp:PlaceHolder>

<asp:PlaceHolder ID="phExitTranslate" runat="server" Visible ="false">
    <asp:HyperLink ID="hlExitTranslate" style="font-size: xx-small; float: right;" runat="server" resourcekey="ExitTranslate" /><br />
</asp:PlaceHolder>

<asp:PlaceHolder ID="plInfo" runat="server" Visible ="false">
    <asp:Label ID="lblInfo" runat="server" resourcekey="Info"></asp:Label>
</asp:PlaceHolder>

<asp:Literal ID="TheText" runat="server"></asp:Literal>

<asp:PlaceHolder ID="phTheText" runat="server"></asp:PlaceHolder>
