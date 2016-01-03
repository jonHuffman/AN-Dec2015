using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Core;

namespace TBD.Views
{
  public class PersistentUIView : BaseView
  {
    private bool _isMuted = false;

    public void UI_OnInfoPressed()
    {
      AppHub.viewManager.AddView(View.Credits);
    }

    public void UI_OnMutePressed()
    {
      _isMuted = !_isMuted;
      AppHub.soundManager.MuteAllSounds(_isMuted);
    }
  }
}