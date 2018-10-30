using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Class which manages the opened wallet ui.
/// </summary>
public sealed partial class OpenWalletMenu : Menu<OpenWalletMenu>
{
    /// <summary>
    /// Class which manages the visuals of the pending transactions.
    /// </summary>
    public sealed class PendingTransactionManager
    {
        private readonly UserWalletManager userWalletManager;
        private readonly EthereumPendingTransactionManager ethereumPendingTransactionManager;
        private readonly EthereumNetworkManager.Settings ethereumNetworkSettings;

        private readonly GameObject pendingTransactionSection;
        private readonly GameObject triangle;
        private readonly TextMeshProUGUI pendingTransactionText, transactionHashText;
        private readonly Button viewOnBrowserButton, walletLogo, exitButton;
        private readonly Image statusIcon;

        private readonly Sprite loadingIconSprite, checkmarkIconSprite, errorIconSprite;

        private readonly LoadingTextAnimator pendingTransactionTextAnimator;
        private readonly LoadingIconAnimator statusIconAnimator;
        private readonly LoadingIconAnimator logoAnimator;

        private string transactionHash;

        private readonly Color FLAT_WHITE = new Color(1f, 1f, 1f, 1f);

        private const string ETHERSCAN_HASH_URL = "https://etherscan.io/tx/";
        private const string RINKEBY_ETHERSCAN_HASH_URL = "https://rinkeby.etherscan.io/tx/";

        public bool PendingTransaction { get; private set; }

        public bool PendingTransactionSectionOpen { get; private set; }

		/// <summary>
		/// Sets the necessary variables needed for the pending transaction section
		/// </summary>
        /// <param name="ethereumNetworkSettings"> The active ethereum network settings. </param>
		/// <param name="ethereumPendingTransactionManager"> The active EthereumPendingTransactionManager. </param>
		/// <param name="userWalletManager"> The active UserWalletManager. </param>
		/// <param name="pendingTransactionSection"> The parent transform of the pending transaction section. </param>
		/// <param name="walletLogo"> The active wallet logo button. </param>
		/// /// <param name="logoutHandler"> The active LogoiutHandler. </param>
		public PendingTransactionManager(
            EthereumNetworkManager.Settings ethereumNetworkSettings,
            EthereumPendingTransactionManager ethereumPendingTransactionManager,
            UserWalletManager userWalletManager,
            Transform pendingTransactionSection,
            Button walletLogo)
        {
            this.ethereumNetworkSettings = ethereumNetworkSettings;
            this.ethereumPendingTransactionManager = ethereumPendingTransactionManager;
            this.userWalletManager = userWalletManager;

            this.pendingTransactionSection = pendingTransactionSection.gameObject;
            this.walletLogo = walletLogo;

            statusIcon = pendingTransactionSection.GetChild(0).GetComponent<Image>();
            statusIconAnimator = statusIcon.GetComponent<LoadingIconAnimator>();
            pendingTransactionText = pendingTransactionSection.GetChild(1).GetComponent<TextMeshProUGUI>();
            pendingTransactionTextAnimator = pendingTransactionText.GetComponent<LoadingTextAnimator>();
            transactionHashText = pendingTransactionSection.GetChild(2).GetComponent<TextMeshProUGUI>();
            viewOnBrowserButton = pendingTransactionSection.GetChild(3).GetComponent<Button>();
            triangle = pendingTransactionSection.GetChild(4).gameObject;
            exitButton = pendingTransactionSection.GetChild(5).GetComponent<Button>();
            exitButton.onClick.AddListener(TransactionPopupExited);
            logoAnimator = walletLogo.GetComponent<LoadingIconAnimator>();

            SetSprite(ref loadingIconSprite, "Loading_Icon");
            SetSprite(ref checkmarkIconSprite, "Checkmark_Icon");
            SetSprite(ref errorIconSprite, "Error_Icon");

            viewOnBrowserButton.onClick.AddListener(ViewOnBrowserClicked);

			ethereumPendingTransactionManager.OnNewTransactionPending += TransactionStarted;
            ethereumPendingTransactionManager.OnTransactionSuccessful += OnTransactionSuccessful;
            ethereumPendingTransactionManager.OnTransactionUnsuccessful += OnTransactionUnsuccessful;
            AccountsPopup.OnAccountChanged += _ => AccountChanged();
		}

        public void Reset()
        {
            ethereumPendingTransactionManager.OnNewTransactionPending -= TransactionStarted;
            ethereumPendingTransactionManager.OnTransactionSuccessful -= OnTransactionSuccessful;
            ethereumPendingTransactionManager.OnTransactionUnsuccessful -= OnTransactionUnsuccessful;
            AccountsPopup.OnAccountChanged += _ => AccountChanged();
        }

        /// <summary>
        /// Sets the target sprite from a given string
        /// </summary>
        /// <param name="targetSprite"> The target sprite being set </param>
        /// <param name="iconName"> The name of the icon file </param>
        private void SetSprite(ref Sprite targetSprite, string iconName)
        {
            Texture2D loadedTexture = Resources.Load("UI/Graphics/Textures/Icons/" + iconName) as Texture2D;
            targetSprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Opens up etherscan to view the info of the transaction hash.
        /// </summary>
        private void ViewOnBrowserClicked()
        {
            Application.OpenURL((ethereumNetworkSettings.networkType == EthereumNetworkManager.NetworkType.Mainnet
                ? ETHERSCAN_HASH_URL
                : RINKEBY_ETHERSCAN_HASH_URL) + transactionHash);
        }

        private void AccountChanged()
        {
            var address = userWalletManager.GetWalletAddress();
            var pendingTransaction = ethereumPendingTransactionManager.GetPendingTransaction(address);

            if ((PendingTransactionSectionOpen && pendingTransaction == null) || (PendingTransactionSectionOpen && !pendingTransaction.isPending && pendingTransaction.result == null))
            {
                AnimatePendingTransactionSection(false);
            }
            else if (!PendingTransactionSectionOpen && pendingTransaction?.isPending == true)
            {
                TransactionStarted(pendingTransaction.txHash, pendingTransaction.message);
            }
            else if (!PendingTransactionSectionOpen && pendingTransaction?.isPending == false && pendingTransaction?.result != null)
            {
                TransactionFinished(pendingTransaction.result == EthereumPendingTransactionManager.PendingTransaction.TransactionResult.Success);
                AnimatePendingTransactionSection(true, false);
            }
            else if (PendingTransactionSectionOpen && ((pendingTransaction?.isPending == true && pendingTransaction?.result == null) || pendingTransaction?.result != null))
            {
                TransactionStarted(pendingTransaction.txHash, pendingTransaction.message);

                if (pendingTransaction.result != null)
                    TransactionFinished(pendingTransaction.result == EthereumPendingTransactionManager.PendingTransaction.TransactionResult.Success);
            }
        }

        private void TransactionPopupExited()
        {
            AnimatePendingTransactionSection(false);
            ethereumPendingTransactionManager.GetPendingTransaction(userWalletManager.GetWalletAddress()).result = null;
        }

        private void OnTransactionSuccessful(string txHash)
        {
            var pendingTransaction = ethereumPendingTransactionManager.GetPendingTransaction(txHash);

            if (!pendingTransaction.addressFrom.EqualsIgnoreCase(userWalletManager.GetWalletAddress()))
                return;

            TransactionFinished(true);
        }

        private void OnTransactionUnsuccessful(string txHash)
        {
            var pendingTransaction = ethereumPendingTransactionManager.GetPendingTransaction(txHash);

            if (!pendingTransaction.addressFrom.EqualsIgnoreCase(userWalletManager.GetWalletAddress()))
                return;

            TransactionFinished(false);
        }

        /// <summary>
        /// Animates the pending transaction section in or out of view
        /// </summary>
        /// <param name="animateIn"> Whether animating in or out </param>
        public void AnimatePendingTransactionSection(bool animateIn, bool animateExit = true)
        {
            float value = animateIn ? 1f : 0f;

            if (animateIn)
                statusIcon.gameObject.SetActive(true);

            pendingTransactionSection.AnimateGraphic(value, 0.2f);
            triangle.AnimateGraphic(value, 0.2f);
            statusIcon.gameObject.AnimateGraphic(value, 0.3f, () => { if (!animateIn) statusIcon.gameObject.SetActive(false); });
            pendingTransactionText.gameObject.AnimateGraphic(value, 0.3f);
            transactionHashText.gameObject.AnimateGraphic(value, 0.4f);
            viewOnBrowserButton.gameObject.AnimateGraphic(value, 0.4f);

            walletLogo.interactable = !animateIn;

            if (animateExit)
            {
                exitButton.gameObject.AnimateGraphic(0f, 0.2f, () => exitButton.gameObject.SetActive(false));
            }

            if (!animateIn)
            {
                logoAnimator.enabled = false;
                walletLogo.gameObject.AnimateColor(FLAT_WHITE, 0.25f);
                walletLogo.interactable = true;
            }

            PendingTransactionSectionOpen = animateIn;
        }

        /// <summary>
        /// New pending transaction has started
        /// </summary>
        /// <param name="message"> The message which represents the transaction being sent. </param>
        /// <param name="transactionHash"> The transaction hash of the pending transaction </param>
        public void TransactionStarted(string transactionHash, string message)
        {
            PendingTransaction = true;

            statusIcon.sprite = loadingIconSprite;
            statusIconAnimator.enabled = true;

            this.transactionHash = transactionHash;
            transactionHashText.text = transactionHash.LimitEnd(17, "...");

            pendingTransactionText.text = message;
            pendingTransactionTextAnimator.enabled = true;

            logoAnimator.enabled = true;

            if (!PendingTransactionSectionOpen)
            {
                AnimatePendingTransactionSection(true);
            }
            else
            {
                exitButton.gameObject.AnimateGraphic(0f, 0.01f);
                exitButton.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// The transaction has finished
        /// </summary>
        /// <param name="successful"> Whether the transaction has finished successfully or not </param>
        public void TransactionFinished(bool successful)
        {
            PendingTransaction = false;

            exitButton.gameObject.SetActive(true);
            exitButton.gameObject.AnimateGraphic(1f, 0.2f);

            pendingTransactionTextAnimator.enabled = false;

            statusIcon.sprite = successful ? checkmarkIconSprite : errorIconSprite;
            statusIconAnimator.enabled = false;
            statusIcon.color = FLAT_WHITE;
            statusIcon.transform.DORotate(Vector3.zero, 0.02f);
            statusIcon.gameObject.AnimateColor(FLAT_WHITE, 1.25f);

            pendingTransactionTextAnimator.enabled = false;
            pendingTransactionText.text = successful ? "Success!" : "Failed.";

            logoAnimator.enabled = false;
            walletLogo.gameObject.AnimateColor(FLAT_WHITE, 1.25f);

            if (!successful)
                walletLogo.interactable = true;
        }
    }
}
