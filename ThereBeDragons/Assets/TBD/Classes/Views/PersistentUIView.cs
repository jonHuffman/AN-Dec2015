using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Core;
using TBD.Events;

namespace TBD.Views
{
  public class PersistentUIView : BaseView
  {
    private bool _isMuted = false;

    public void UI_OnInfoPressed()
    {
      AppHub.viewManager.AddView(View.Credits);

      //Reset everything. If the player opens this in the middle of gameplay they are going to die anyway.
      AppHub.eventManager.Dispatch(GameEvent.Restart);
    }

    public void UI_OnMutePressed()
    {
      _isMuted = !_isMuted;
      AppHub.soundManager.MuteAllSounds(_isMuted);
    }
  }
}