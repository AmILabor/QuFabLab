/// <summary>
/// Verwaltet das Dialogfenster, das beim Untersuchen eines Bausteins erscheint.
/// Zeigt Typinformationen, Einstellungsoptionen und Aktionsbuttons (Löschen, Rotieren, Typ ändern) an.
/// </summary>
using System.Collections;
using AMI.Util;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using QuantenKoffer.Bricks;
using QuantenKoffer.Case;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using Util;

namespace QuantenKoffer.Dialog
{
    /// <summary>
    /// Klasse, die alles Verhalten hält, bezüglich des erscheinenden Fensters, wenn ein Spielstein untersucht wird.
    /// Ein Objekt dieser Klasse bzw. das Skript ist an das GameObjekt Dialog angehangen.
    /// </summary>
    public class DialogHandler : MonoBehaviour, INextFrameUnityEventInvoker
    {
        private Brick brick;
        //!< Verweis auf den zu untersuchenden Stein

        [SerializeField] public float yOffset = 1.0f;
        //!< lässt den Dialog über den Stein erscheinen

        [SerializeField] public GameObject InfoDialog;
        //!< Child-Objekt vom Dialog, beinhaltet Beschreibung, Typname und Icon

        [SerializeField] public GameObject OptionsDialog;
        //!< Child-Objekt vom Dialog, beinhaltet alle Einstellungsoptionen, \see QuantenKoffer::Brick

        [SerializeField] public GameObject Buttons;
        //!< Child-Objekt vom Dialog, beinhaltet alle Menu-Buttons (refactoring) zu MenuButtons umbennenen

        [SerializeField] public GameObject ExitButton;
        //!< Child-Objekt vom Dialog, beinhaltet den Exit-Button

        [SerializeField] public GameObject DeleteButton;
        //!< Child-Objekt vom Dialog/Menu_Buttons/ButtonCollection, beinhaltet den DeleteButton 

        [SerializeField] public GameObject BrickFactory;
        //!< Referenz auf die BrickFactory um den Typ zu ändern, \see ChangeType(string type)

        [SerializeField] public UnityEvent<Brick> OnBrickAction;
        //!< \see QuantenKoffer::Bricks::INextFrameUnityEventInvoker 

        private BrickFactory factory;
        //!< \see ChangeType(string type)

        private PressableButtonHoloLens2[] bottomButtons;


        /// <summary>
        /// Setzt factory und buttons.
        /// </summary>
        private void Awake()
        {
            factory = BrickFactory.GetComponent<BrickFactory>();
            bottomButtons = Buttons.GetComponentsInChildren<PressableButtonHoloLens2>();
        }

        /// <summary>
        /// Setzt Fenster über den Brick
        /// </summary>
        private void Update()
        {
            if (brick != null)
            {
                var position = brick.transform.position;
                position.y += yOffset;
                transform.position = position;
            }
        }

        /// <summary>
        /// Ändert den Typen des Bricks über ein Button im Dialog-Fenster.
        /// </summary>
        /// 
        /// <details>
        /// Wenn ein Brick vorher auf dem Grid war, wird der neue Grid auch auf das Grid gepackt. Wenn das nicht der
        /// Fall ist, nämlich nur dann wer auf dem Spawn liegt, soll das nicht geschehen. Allgemein wird die
        /// Typänderung durch ein Löschen und Neuerstellen des Steins verwirklicht. Aktualisiert die class-property
        /// brick.
        /// </details>
        /// <param name="type">Der Typ des Bricks als String</param>
        // (refactoring) Das Replacen des Bricks mit einem anderen neues Typen kann man vielleicht durch Operator-
        // overloading oder durch eine Funktion in Brick handhaben. Die Funktionalität ist aufjedenfalls nicht 
        // hier gut aufgehoben.
        public void ChangeType(string type)
        {
            GameObject ob = null;
            switch (type)
            {
                //Ugly workaround because unity inspector does not allow enums as arguments for onlcick.
                case "Mirror45":
                    ob = factory.CreateMirror45();
                    break;
                case "Mirror90":
                    ob = factory.CreateMirror90();
                    break;
                case "BeamSplitter":
                    ob = factory.CreateBeamSplitter();
                    break;
            }

            if (ob == null) Console.Log("DialogHandler/ChangeType", "Could not change Type to " + type);
            GridDirection rotation_orig = brick.GetRotation();
            ob.transform.position = brick.transform.position;
            ob.transform.rotation = brick.transform.rotation;
            bool wasOnGrid = brick.IsOnGrid();
            brick.Destroy();
            brick = ob.GetComponentInChildren<Brick>();
            if (wasOnGrid)
                brick.gameObject.SendMessageUpwards("SnapToGrid", brick);
            // brick.RotateTo(rotation_orig); // does not work
            
            // Test This!!!!!!!!!!!!!!!!!!!!!!
            brick.SetRotation(rotation_orig);
            

            updateDialog();
            StartCoroutine(((INextFrameUnityEventInvoker)this).InvokeNextFrame<Brick>(OnBrickAction, brick));
            // Soll alle Funktionen, die unter onBrickAction liegen im nächsten Frame ausführen.
        }

        /// <summary>
        /// Nimmt den Typen des aktualisierten Brick und verwendet den um die Beschreibung dessen zu laden.
        /// </summary>
        ///
        /// <details>
        /// Wird aufgerufen, wenn der Stein oben berührt wird und wenn der Typ über den jeweiligen Button
        /// geändert wird.
        /// </details>
        private void updateDialog()
        {
            var type = brick.GetBrickType();
            var description = InfoDialog.GetNamedChild("DescriptionText");
            var name = InfoDialog.GetNamedChild("Name");
            var icon = InfoDialog.GetNamedChild("Image");
            var tmp = description.GetComponent<TextMeshPro>();
            var tmpn = name.GetComponent<TextMeshPro>();
            tmp.text = type.description.GetLocalizedString();
            tmpn.text = type.brickName.GetLocalizedString();
            icon.GetComponent<SpriteRenderer>().sprite = type.sprite;
            if (type.TypeEnum != QuBrickTypeEnum.Mirror90)
            {
                foreach (var button in bottomButtons)
                {
                    if (button.gameObject.name == "Button Options")
                    {
                        button.gameObject.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// Brick registriert sich, aktiviert den InfoDialog und pullt durch UpdateDialog()
        /// die Informationen zum Typen.
        /// </summary>
        /// <param name="_brick">Der aufrufende Brick</param>
        public void SetCurrentBrick(Brick _brick)
        {
            brick = _brick;
            gameObject.SetChildrenActive(false);
            InfoDialog.SetActive(true);
            Buttons.SetActive(true);
            EnableAllNavigationButtons();
            ExitButton.SetActive(true);
            updateDialog();
        }

        /// <summary>
        /// Setzt den Wert von Slidern auf den Setting-Wert, den der Brick hat.
        /// </summary>
        ///
        /// <details>
        /// Nur Brick90 hat Settings. 
        /// </details>
        // (refactoring) über Mix-In Settings regeln
        private void ApplyCurrentBrickSetting()
        {
            GetComponentInChildren<PinchSlider>().SliderValue = brick.GetSetting();
        }

        /// <summary>
        /// Läuft automatisch, sobald der Dialog angezeigt wird über den onClick-Handler.
        /// \see PollSettingCoroutine()
        /// </summary>
        public void PollSettingsFromCurrentBrick()
        {
            StartCoroutine(PollSettingCoroutine());
        }

        /// <summary>
        /// Solange der Dialog aktiv ist (sichtbar), aktualisiere die Setting-Werte.
        /// </summary>
        ///
        /// <details>
        /// Asynchrone Visualisierung der Settings für den Websocket. 
        /// </details>
        /// <returns></returns>
        private IEnumerator PollSettingCoroutine()
        {
            while (OptionsDialog.activeSelf)
            {
                ApplyCurrentBrickSetting();
                yield return new WaitForSeconds(1);
            }
        }

        //(refactoring) SetStateNavigationButtons statt zwei Methoden enable und disable
        public void DisableNavigationButtons()
        {
            Buttons.SetActive(false);
        }

        /// <summary>
        /// Deaktiviert alle Buttons außer den Delete-Button.
        /// </summary>
        ///
        /// <details>
        /// Für feste Bestandteile des Experiments gedacht, wie z.B den Periskop.
        /// </details>
        public void DisableNavigationButtonsButDelete()
        {
            Buttons.SetActive(true);
            foreach (var button in bottomButtons)
                if (button.transform.gameObject != DeleteButton)
                    button.gameObject.SetActive(false);
        }

        /// <summary>
        /// \see DisableNavigationButtonsButDelete()
        /// </summary>
        private void EnableAllNavigationButtons()
        {
            Buttons.SetActive(true);
            foreach (var button in bottomButtons)
                button.gameObject.SetActive(true);
        }

        /// <summary>
        /// handler für onClick-Handler bei Delete-Button
        /// </summary>
        // (refactoring) Schreibe eine Methode um den Dialog ein- und auszublenden
        public void DestroyCurrentBrick()
        {
            if (brick.IsOnGrid())
                brick.SendMessageUpwards("RemoveFromGrid", brick, SendMessageOptions.DontRequireReceiver);
            else
                brick.Destroy();
            InfoDialog.SetActive(false);
            Buttons.SetActive(false);
            ExitButton.SetActive(false);
        }

        /// <summary>
        /// handler für onClick-Handle rotateLeftButton
        /// </summary>
        public void RotateCurrentBrickLeft()
        {
            brick.RotateLeft();
            GridDirection rotation = brick.GetRotation();
            // updateDialog(); // this did nothing
            StartCoroutine(((INextFrameUnityEventInvoker)this).InvokeNextFrame<Brick>(OnBrickAction, brick));
        }

        /// <summary>
        /// handler für onClick-Handle rotateRightButton
        /// </summary>
        public void RotateCurrentBrickRight()
        {
            brick.RotateRight();
            // updateDialog(); // this did nothing
            StartCoroutine(((INextFrameUnityEventInvoker)this).InvokeNextFrame<Brick>(OnBrickAction, brick));
        }

        /// <summary>
        /// handler für onClick-Handle rotateLeftButton
        /// </summary>
        ///
        /// <details>
        /// Speichert Rechenzeit, indem nicht ApplySetting aufrufen, wenn es nicht sein soll.
        /// </details>
        public void AdjustDistanceSetting(SliderEventData setting)
        {
            if (setting.NewValue != setting.OldValue)
                brick.ApplySetting(setting.NewValue);
        }
    }
}