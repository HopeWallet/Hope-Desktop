using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public sealed partial class OpenWalletMenu : Menu<OpenWalletMenu>
{
	public sealed class PendingTransactionManager
	{
		private GameObject pendingTransactionSection;
		private GameObject triangle;
		private TextMeshProUGUI pendingTransactionText, transactionHashText;
		private Button copyButton, walletLogo;
		private GameObject checkmarkIconObject;
		private Image statusIcon;

		private Sprite loadingIconSprite, checkmarkIconSprite, errorIconSprite;

		private LoadingTextAnimator pendingTransactionTextAnimator;
		private LoadingIconAnimator statusIconAnimator;
		private LoadingIconAnimator logoAnimator;

		private string transactionHash;
		private bool animatingIcon, pendingTransaction;

		private readonly Color FLAT_WHITE = new Color(1f, 1f, 1f);

		public PendingTransactionManager(Transform pendingTransactionSection,
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
			this.pendingTransactionSection = pendingTransactionSection.gameObject;
			this.walletLogo = walletLogo;
			logoAnimator = walletLogo.GetComponent<LoadingIconAnimator>();

			SetSprite(ref loadingIconSprite, "Loading_Icon");
			SetSprite(ref checkmarkIconSprite, "Checkmark_Icon");
			SetSprite(ref errorIconSprite, "Error_Icon");

			copyButton.onClick.AddListener(CopyButtonClicked);
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

		private void AnimatePendingTransactionSection(bool animateIn)
		{
			float value = animateIn ? 1f : 0f;

			pendingTransactionSection.AnimateGraphic(value, animateIn ? 0.2f : 0.4f);
			triangle.AnimateGraphic(value, animateIn ? 0.2f : 0.4f);
			statusIcon.gameObject.AnimateGraphic(value, animateIn ? 0.3f : 0.4f);
			pendingTransactionText.gameObject.AnimateGraphic(value, animateIn ? 0.3f : 0.4f);
			transactionHashText.gameObject.AnimateGraphic(value, 0.4f);
			copyButton.gameObject.AnimateGraphic(value, 0.4f);
		}

		public void TransactionStarted(string action, string assetSymbol, string transactionHash)
		{
			pendingTransaction = true;
			this.transactionHash = transactionHash;

			statusIcon.sprite = loadingIconSprite;
			statusIconAnimator.enabled = true;

			transactionHashText.text = transactionHash.LimitEnd(12, "...");

			pendingTransactionText.text = action + " " + assetSymbol;
			pendingTransactionTextAnimator.enabled = true;

			walletLogo.interactable = false;
			logoAnimator.enabled = true;

			AnimatePendingTransactionSection(true);
		}

		public void TransactionFinished(bool successful)
		{
			pendingTransaction = false;
			pendingTransactionTextAnimator.enabled = false;

			statusIcon.sprite = successful ? checkmarkIconSprite : errorIconSprite;
			statusIconAnimator.enabled = false;
			statusIcon.color = FLAT_WHITE;
			statusIcon.transform.DORotate(Vector3.zero, 0.02f);
			statusIcon.gameObject.AnimateColor(FLAT_WHITE, 1f);

			pendingTransactionTextAnimator.enabled = false;
			CoroutineUtils.ExecuteAfterWait(0.2f, () => pendingTransactionText.text = successful ? "Success!" : "Failed.");

			walletLogo.interactable = true;
			logoAnimator.enabled = false;
			walletLogo.gameObject.AnimateColor(FLAT_WHITE, 1f);

			CloseAfterWait().StartCoroutine();
		}

		private IEnumerator CloseAfterWait()
		{
			yield return new WaitForSeconds(5f);

			if (!pendingTransaction)
				AnimatePendingTransactionSection(false);
		}
	}
}
