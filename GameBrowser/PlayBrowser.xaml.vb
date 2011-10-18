﻿Public Class PlayBrowser
    Private m_recentItems As RecentItems
    Private WithEvents m_onlineGames As New OnlineGames

    Public Event LaunchGame(filename As String)
    Public Event GotUpdateData(data As UpdatesData)

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        m_recentItems = New RecentItems("Recent")
        ctlGameList.LaunchCaption = "Play"
        ctlOnlineGameList.LaunchCaption = "Play"
        ctlOnlineGameList.IsOnlineList = True
        Populate()
    End Sub

    Public Sub AddToRecent(filename As String, name As String)
        m_recentItems.AddToRecent(filename, name)
    End Sub

    Private Sub ctlGameList_Launch(filename As String) Handles ctlGameList.Launch
        RaiseEvent LaunchGame(filename)
    End Sub

    Private Sub ctlGameList_ClearAllItems() Handles ctlGameList.ClearAllItems
        m_recentItems.Clear()
        Populate()
    End Sub

    Private Sub ctlGameList_RemoveItem(filename As String) Handles ctlGameList.RemoveItem
        m_recentItems.Remove(filename)
    End Sub

    Private Sub ctlOnlineGameList_Launch(filename As String) Handles ctlOnlineGameList.Launch
        RaiseEvent LaunchGame(filename)
    End Sub

    Public Sub Populate()
        m_recentItems.PopulateGameList(ctlGameList)
    End Sub

    Public Sub MainWindowShown()
        m_onlineGames.StartDownloadGameData()
    End Sub

    Private Sub m_onlineGames_DataReady() Handles m_onlineGames.DataReady
        Dispatcher.BeginInvoke(Sub() PopulateCategories())
    End Sub

    Private Sub m_onlineGames_DownloadFailed() Handles m_onlineGames.DownloadFailed
        Dispatcher.BeginInvoke(Sub() ctlOnlineGameList.MarkAsFailed())
    End Sub

    Private Sub PopulateCategories()
        ctlBrowseFilter.Populate((From cat In m_onlineGames.Categories Select cat.Title).ToArray())
    End Sub

    Private Sub ctlBrowseFilter_CategoryChanged(category As String) Handles ctlBrowseFilter.CategoryChanged
        PopulateGames(category)
    End Sub

    Private Sub PopulateGames(category As String)
        m_onlineGames.PopulateGameList(category, ctlOnlineGameList)
    End Sub

    Private Sub m_onlineGames_GotUpdateData(data As UpdatesData) Handles m_onlineGames.GotUpdateData
        RaiseEvent GotUpdateData(data)
    End Sub

    Private Sub ctlOnlineGameList_ShowGameDescription(id As String) Handles ctlOnlineGameList.ShowGameDescription
        SetDescriptionVisible(True)
    End Sub

    Private Sub SetDescriptionVisible(visible As Boolean)
        Dim gameListVisibility As Windows.Visibility = If(visible, Windows.Visibility.Collapsed, Windows.Visibility.Visible)
        Dim descriptionVisibility As Windows.Visibility = If(visible, Windows.Visibility.Visible, Windows.Visibility.Collapsed)
        lblRecent.Visibility = gameListVisibility
        ctlGameList.Visibility = gameListVisibility
        ctlGameDescription.Visibility = descriptionVisibility
    End Sub

    Private Sub ctlGameDescription_Close() Handles ctlGameDescription.Close
        SetDescriptionVisible(False)
    End Sub
End Class
