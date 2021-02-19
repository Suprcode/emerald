using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using Network = Emerald.Network;
using C = ClientPackets;
using Toggle = UnityEngine.UI.Toggle;

public class PartyController : MonoBehaviour {
    [SerializeField] private Toggle allowGroupToggle;
    [SerializeField] private TMP_InputField inputPlayerName;
    [SerializeField] private GameObject inviteWindow;
    [SerializeField] private TMP_Text inviteLabel;
    [SerializeField] private GameObject memberSlot;
    [SerializeField] private GameObject memberContainer;
    [SerializeField] private GameObject uiMemberContainer;
    [SerializeField] private GameObject uiMemberSlot;
    [SerializeField] private GameObject uiCollapsePartyButton;
    [SerializeField] private TextMeshProUGUI pageCountText;
    [SerializeField] private GameObject groupPage;
    [SerializeField] private GameObject receiveInviteWindow;
    [SerializeField] private GameObject deleteMemberWindow;
    private int currentPage;
    private readonly List<string> partyList = new List<string>();
    private readonly List<GameObject> memberSlots = new List<GameObject>();
    private readonly List<GameObject> uiMemberSlots = new List<GameObject>();
    
    /* TODO: Checks before sending package */
    /* TODO: Optomise, only delete member when deleted, don't remake the full list*/
    /* TODO: Packet receiver for initial allow group value or find where this is already sent
        Careful with the ChangeAllowGroupValue and recursive loop.*/
    
    public string UserName { get; set; }

    public void ChangeAllowGroupValue() {
        Network.Enqueue(new C.SwitchGroup { AllowGroup = allowGroupToggle.isOn});
    }

    public void LeaveParty() {
        ClearPartyListAndMemberSlots();
        Network.Enqueue(new C.DelMember() {Name = UserName});
    }

    public void RemoveMemberFromParty() {
        Network.Enqueue(new C.DelMember { Name = inputPlayerName.text});
    }

    public void RemoveMemberFromParty(string playerName) {
        Network.Enqueue(new C.DelMember { Name = playerName});
    }

    public void RemoveMemberFromParty(int memberPosition) {
        Network.Enqueue(new C.DelMember() { Name = partyList[memberPosition]});
    }

    public void SendInviteToPlayer() {
        if (!RoomForMorePlayers()) return;
        if (!IsPartyLeader()) return;
        if (inputPlayerName.text.Length <= 3) return;
        Network.Enqueue(new C.AddMember() { Name = inputPlayerName.text });
        inputPlayerName.text = "";
    }

    public void ReplyToPartyInvite(bool response) {
        Network.Enqueue(new C.GroupInvite() { AcceptInvite = response });
        inviteWindow.SetActive(false);
    }

    public void ReceiveInvite(string fromPlayer) {
        receiveInviteWindow.transform.GetChild(2).
            GetComponent<TextMeshProUGUI>().SetText($"{fromPlayer} has invited you to join their group");
        receiveInviteWindow.SetActive(true);
    }

    public void AddToPartyList(string newMember) {
        Debug.Log("AddToPartyList");
        partyList.Add(newMember);
        RefreshPartyMenu();
    }


    public void RemoveFromPartyList(string member) {
        Debug.Log("RemoveFromPartyList");
        partyList.Remove(member);
        if (partyList.Count == 1) {
            RemoveFromPartyList(UserName);
        }
        RefreshPartyMenu();
    }

    public void ClearPartyListAndMemberSlots() {
        ClearMemberSlots();
        partyList.Clear();
        SetUiCollapseButtonActive();
    }

    private void ClearMemberSlots() {
        for (int i = 0; i < memberSlots.Count; i++) {
            Destroy(memberSlots[i]);
            Destroy(uiMemberSlots[i]);
        }
        memberSlots.Clear();
        uiMemberSlots.Clear();
    }

    private void RefreshPartyMenu() {
        ClearMemberSlots();
        GameObject currentContainer = SetNewPartyPage();
        for (int i = 0; i < partyList.Count; i++) {
            if (i != 0 || i % 5 == 0) {
                currentContainer = SetNewPartyPage();
            }
            SetPartyMemberSlots(memberSlot, currentContainer, memberSlots, i);
            // SetPartyMemberSlots(uiMemberSlot, uiMemberContainer, uiMemberSlots, i);
        }
        SetUiCollapseButtonActive();
    }

    private GameObject SetNewPartyPage() {
        return Instantiate(groupPage, memberContainer.transform);
    }

    private void SetUiCollapseButtonActive() {
        uiCollapsePartyButton.SetActive(partyList.Count > 0);
    }

    private void SetPartyMemberSlots(GameObject slot, GameObject parent, List<GameObject> slotList, int position) {
        slotList.Add(Instantiate(slot, parent.transform));
        GameObject kickButton = slotList[position].transform.GetChild(1).gameObject;
        GameObject nameTextField = slotList[position].transform.GetChild(0).GetChild(1).gameObject;
        nameTextField.GetComponent<TextMeshProUGUI>().SetText(partyList[position]);
        kickButton.AddComponent<PlayerSlot>().Construct(this, partyList[position], kickButton, ShouldShowKickButton(position));
    }

    private bool ShouldShowKickButton(int position) => position > 0 && partyList[0] == UserName;

    private bool IsPartyLeader() {
        return partyList.Count == 0 || UserName == partyList[0];
    }

    private bool RoomForMorePlayers() {
        return partyList.Count < 5; // Global party count?
    }
}

internal class PlayerSlot : MonoBehaviour {

    public void Construct(PartyController partyController, string playerName, GameObject kickButton,
        bool shouldShowKickButton) {

        if(shouldShowKickButton)
            kickButton.GetComponent<Button>().onClick.AddListener(() 
                => partyController.RemoveMemberFromParty(playerName));
        else 
            kickButton.SetActive(false);
    }
}
