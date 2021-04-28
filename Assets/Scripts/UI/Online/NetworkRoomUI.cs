using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Online
{
    public class NetworkRoomUI : MonoBehaviourPunCallbacks
    {
        private Text _playerName;

        private void Start()
        {
            _playerName = transform.parent.parent.parent.parent.Find("PlayerName").Find("Text").GetComponent<Text>();
        }

        public void JoinRoom()
        {
            if (!PhotonNetwork.IsConnected || PhotonNetwork.InRoom)
                return;
            PhotonNetwork.LocalPlayer.NickName = _playerName.text;
            PhotonNetwork.JoinRoom(gameObject.name);
            var roomText = transform.Find("Text").GetComponent<Text>();
            roomText.color = new Color(1f, 0.34f, 0.44f);
        }
    }
}