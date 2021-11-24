using Godot;

public class ShopUI : Control
{
	[Export]
	public Texture buyTexture;

	[Export]
	public Texture ownedTexture;

	[Export]
	public Texture usingTexture;

	private Label moneyLabel;
	private TextureButton[] weaponButtons;
	private TextureRect darkSmoke1Node;
	private TextureRect darkSmoke2Node;
	private int[] weaponCosts = new int[5] { 0, 25, 35, 60, 90 };
	private bool escapePressed = false;

	public override void _Ready()
	{
		weaponButtons = new TextureButton[Player.AmountOfGuns];
		weaponButtons[Player.Gun_Phaser] = GetNode<TextureButton>("Shop/PhaserButton");
		weaponButtons[Player.Gun_Blaster] = GetNode<TextureButton>("Shop/BlasterButton");
		weaponButtons[Player.Gun_Boomer] = GetNode<TextureButton>("Shop/BoomerButton");
		weaponButtons[Player.Gun_PhaseRifle] = GetNode<TextureButton>("Shop/PhaserRifleButton");
		weaponButtons[Player.Gun_DoomCannon] = GetNode<TextureButton>("Shop/Doom-CannonButton");
		darkSmoke1Node = GetNode<TextureRect>("Background/DarkSmoke_1");
		darkSmoke2Node = GetNode<TextureRect>("Background/DarkSmoke_2");
		moneyLabel = GetNode<Label>("MoneyCounter/MoneyLabel");
		ResetAllButtonIcons();
	}

	public override void _Process(float delta)
	{
		for (int i = 0; i < Player.AmountOfGuns; i++)
		{
			weaponButtons[i].Modulate = new Color(1f, 1f, 1f, 1f);
			if (weaponButtons[i].Pressed)
				weaponButtons[i].Modulate = new Color(0.6f, 0.6f, 0.6f, 1f);
		}

		if (Input.IsKeyPressed((int)KeyList.Escape))
		{
			Transitions.FadeIn();
			escapePressed = true;
		}
		if (escapePressed && Transitions.fadeInCompleted)
		{
			Transitions.FadeOut();
			ScenesHolder.SwitchScenesTo(ScenesHolder.UI_TitleScreen);
		}

		if (darkSmoke1Node.RectGlobalPosition.x <= -80f)
		{
			darkSmoke1Node.RectGlobalPosition = Vector2.Zero;
		}
		if (darkSmoke2Node.RectGlobalPosition.x >= 320f)
		{
			darkSmoke2Node.RectGlobalPosition = Vector2.Zero;
		}
	}

	private void ResetAllButtonIcons()
	{
		for (int i = 0; i < Player.AmountOfGuns; i++)
		{
			weaponButtons[i].SelfModulate = new Color(1f, 1f, 1f, 1f);
			TextureRect priceTexture = weaponButtons[i].GetNode<TextureRect>("CostTexture");
			if (Player.gunUnlocked[i])
			{
				weaponButtons[i].TextureNormal = ownedTexture;
				weaponButtons[i].TexturePressed = ownedTexture;
				priceTexture.Visible = false;
				if (Player.activeGun == i)
				{
					weaponButtons[i].TextureNormal = usingTexture;
					weaponButtons[i].TexturePressed = usingTexture;
				}
			}
			else
			{
				weaponButtons[i].TextureNormal = buyTexture;
				weaponButtons[i].TexturePressed = buyTexture;
				Label priceLabel = priceTexture.GetNode<Label>("CostLabel");
				priceLabel.Text = weaponCosts[i].ToString();
				if (weaponCosts[i] > Player.playerMoney)
					priceLabel.Modulate = new Color(1f, 0f, 0f, 1f);
			}

		}
		moneyLabel.Text = Player.playerMoney.ToString();
	}

	private void OnPhaserButtonPressed()
	{
		int buttonIndex = Player.Gun_Phaser;

		if (Player.playerMoney < weaponCosts[buttonIndex])
			return;

		if (Player.gunUnlocked[buttonIndex])
		{
			Player.activeGun = buttonIndex;
			ResetAllButtonIcons();
			return;
		}

		Player.playerMoney -= weaponCosts[buttonIndex];
		Player.gunUnlocked[buttonIndex] = true;
		ResetAllButtonIcons();
	}


	private void OnBlasterButtonPressed()
	{
		int buttonIndex = Player.Gun_Blaster;

		if (Player.playerMoney < weaponCosts[buttonIndex])
			return;

		if (Player.gunUnlocked[buttonIndex])
		{
			Player.activeGun = buttonIndex;
			ResetAllButtonIcons();
			return;
		}

		Player.playerMoney -= weaponCosts[buttonIndex];
		Player.gunUnlocked[buttonIndex] = true;
		ResetAllButtonIcons();
	}


	private void OnBoomerButtonPressed()
	{
		int buttonIndex = Player.Gun_Boomer;

		if (Player.playerMoney < weaponCosts[buttonIndex])
			return;

		if (Player.gunUnlocked[buttonIndex])
		{
			Player.activeGun = buttonIndex;
			ResetAllButtonIcons();
			return;
		}

		Player.playerMoney -= weaponCosts[buttonIndex];
		Player.gunUnlocked[buttonIndex] = true;
		ResetAllButtonIcons();
	}


	private void OnPhaserRifleButtonPressed()
	{
		int buttonIndex = Player.Gun_PhaseRifle;

		if (Player.playerMoney < weaponCosts[buttonIndex])
			return;

		if (Player.gunUnlocked[buttonIndex])
		{
			Player.activeGun = buttonIndex;
			ResetAllButtonIcons();
			return;
		}

		Player.playerMoney -= weaponCosts[buttonIndex];
		Player.gunUnlocked[buttonIndex] = true;
		ResetAllButtonIcons();
	}


	private void OnDoomCannonButtonPressed()
	{
		int buttonIndex = Player.Gun_DoomCannon;

		if (Player.playerMoney < weaponCosts[buttonIndex])
			return;

		if (Player.gunUnlocked[buttonIndex])
		{
			Player.activeGun = buttonIndex;
			ResetAllButtonIcons();
			return;
		}

		Player.playerMoney -= weaponCosts[buttonIndex];
		Player.gunUnlocked[buttonIndex] = true;
		ResetAllButtonIcons();
	}
}
