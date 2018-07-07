using UnityEngine;
using UnityEngine.UI;

public class OpenWalletForm : MonoBehaviour {

	[SerializeField] private GameObject token1;
	[SerializeField] private GameObject token2;
	[SerializeField] private GameObject token3;

	private void Start()
	{
		token1.GetComponent<Button>().onClick.AddListener(Button1Clicked);
		token2.GetComponent<Button>().onClick.AddListener(Button2Clicked);
		token3.GetComponent<Button>().onClick.AddListener(Button3Clicked);
	}

	private void Button1Clicked()
	{
		token1.GetComponent<Button>().interactable = false;
		token2.GetComponent<Button>().interactable = true;
		token3.GetComponent<Button>().interactable = true;
	}

	private void Button2Clicked()
	{
		token1.GetComponent<Button>().interactable = true;
		token2.GetComponent<Button>().interactable = false;
		token3.GetComponent<Button>().interactable = true;
	}

	private void Button3Clicked()
	{
		token1.GetComponent<Button>().interactable = true;
		token2.GetComponent<Button>().interactable = true;
		token3.GetComponent<Button>().interactable = false;
	}
}
