using System;

[Serializable]
public class ContactInfoJson
{

	public string address;
	public string name;

	/// <summary>
	/// Initializes the ContactInfoJson by assigning all the values
	/// </summary>
	/// <param name="address"> The contact address </param>
	/// <param name="name"> The contact name </param>
	public ContactInfoJson(string address, string name)
	{
		this.address = address;
		this.name = name;
	}
}
