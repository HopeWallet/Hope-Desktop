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
	/// Class which manages the visuals of the pending transactions
	/// </summary>
	public sealed class PendingTransactionManager
	{
		private readonly GameObject pendingTransactionSection;
		private readonly GameObject triangle;
		private readonly TextMeshProUGUI pendingTransactionText, transactionHashText;
		private readonly Button copyButton, walletLogo, exitButton;
		private readonly GameObject checkmarkIconObject;
		private readonly Image statusIcon;

		private readonly Sprite loadingIconSprite, checkmarkIconSprite, errorIconSprite;

		private readonly LoadingTextAnimator pendingTransactionTextAnimator;
		private readonly LoadingIconAnimator statusIconAnimator;
		private readonly LoadingIconAnimator logoAnimator;

		private string transactionHash;
		private bool animatingIcon;

		public bool PendingTransaction { get; private set; }

		public bool PendingTransactionSectionOpen { get; private set; }

		private readonly Color FLAT_WHITE = new Color(1f, 1f, 1f, 1f);

        /// <summary>
        /// Sets the necessary variables needed for the pending transaction section
        /// </summary>
        /// <param name="ethereumPendingTransactionManager"> The active EthereumPendingTransactionManager. </param>
        /// <param name="pendingTransactionSection"> The parent transform of the pending transaction section </param>
        /// <param name="walletLogo"> The active wallet logo button</param>
        public PendingTransactionManager(
            EthereumPendingTransactionManager ethereumPendingTransactionManager,
            Transform pendingTransactionSection,
            Button walletLogo)
        {
			statusIcon = pendingTransactionSection.GetChild(0).GetComponent<Image>();
			statusIconAnimator = statusIcon.GetComponent<LoadingIconAnimator>();
			pendingTransactionText = pendingTransactionSection.GetChild(1).GetComponent<TextMeshProUGUI>();
			pendingTransactionTextAnimator = pendingTransactionText.GetComponent<LoadingTextAnimator>();
			transactionHashText = pendingTransactionSection.GetChild(2).GetComponent<TextMeshProUGUI>();
			copyButton = pendingTransactionSection.GetChild(3).GetComponent<Button>();
			checkmarkIconObject = pendingTransactionSection.GetChild(4).gameObject;
			triangle = pendingTransactionSection.GetChild(5).gameObject;
			exitButton = pendingTransactionSection.GetChild(6).GetComponent<Button>();
			exitButton.onClick.AddListener(() => AnimatePendingTransactionSection(false));
			this.pendingTransactionSection = pendingTransactionSection.gameObject;
			this.walletLogo = walletLogo;
			logoAnimator = walletLogo.GetComponent<LoadingIconAnimator>();

			SetSprite(ref loadingIconSprite, "Loading_Icon");
			SetSprite(ref checkmarkIconSprite, "Checkmark_Icon");
			SetSprite(ref errorIconSprite, "Error_Icon");

			copyButton.onClick.AddListener(CopyButtonClicked);

            ethereumPendingTransactionManager.OnNewTransactionPending += TransactionStarted;
            ethereumPendingTransactionManager.OnTransactionSuccessful += () => TransactionFinished(true);
            ethereumPendingTransactionManager.OnTransactionUnsuccessful += () => TransactionFinished(false);
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
		/// Copy button is clicked and copies the transaction hash to the clipboard
		/// </summary>
		private void CopyButtonClicked()
		{
			ClipboardUtils.CopyToClipboard(transactionHash);

			if (!animatingIcon)
			{
				animatingIcon = true;
				checkmarkIconObject.transform.localScale = new Vector3(0, 0, 1);

				checkmarkIconObject.AnimateGraphicAndScale(1f, 1f, 0.15f);
				CoroutineUtils.ExecuteAfterWait(0.6f, () => checkmarkIconObject.AnimateGraphic(0f, 0.25f, () => animatingIcon = false));
			}
		}

		/// <summary>
		/// Animates the pending transaction section in or out of view
		/// </summary>
		/// <param name="animateIn"> Whether animating in or out </param>
		public void AnimatePendingTransactionSection(bool animateIn)
		{
			float value = animateIn ? 1f : 0f;

			pendingTransactionSection.AnimateGraphic(value, 0.2f);
			triangle.AnimateGraphic(value, 0.2f);
			statusIcon.gameObject.AnimateGraphic(value, 0.3f);
			pendingTransactionText.gameObject.AnimateGraphic(value, 0.3f);
			transactionHashText.gameObject.AnimateGraphic(value, 0.4f);
			copyButton.gameObject.AnimateGraphic(value, 0.4f);

			walletLogo.interactable = !animateIn;

			exitButton.gameObject.AnimateGraphic(0f, 0.2f, () => exitButton.gameObject.SetActive(false));

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
			transactionHashText.text = transactionHash.LimitEnd(12, "...");

			pendingTransactionText.text = message;
			pendingTransactionTextAnimator.enabled = true;

			logoAnimator.enabled = true;

			AnimatePendingTransactionSection(true);
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
