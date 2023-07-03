using System;
using UniMob;
using UnityEngine.UIElements;

class Login : LifetimeVisualElement
{
    public new class UxmlFactory : UxmlFactory<Login> { }

    [Atom]
    private string Message { get; set; }

    private string Username { get; set; }
    private string Password { get; set; }

    public ViewStore Store { get; set; }
    public Action AfterLogin { get; set; }

    public Login()
    {
        
    }

    protected override void OnInit(Lifetime lifetime)
    {
        var messageLabel = this.Q<Label>("MessageLabel");
        Atom.Reaction(lifetime, () => Message, x => messageLabel.text = x);

        this.Q<TextField>("UsernameTextField").RegisterValueChangedCallback(e =>
        {
            Username = e.newValue;
        });

        this.Q<TextField>("PasswordTextField").RegisterValueChangedCallback(e =>
        { Password = e.newValue; });

        this.Q<Button>().clicked += () =>
        {
            Message = "Verifying credentials...";
            Store.PerformLogin(Username, Password, isOk =>
            {
                Message = isOk ? "Login accepted" : "Login failed";

                if (isOk)
                    AfterLogin();
            });

        };
    }

}
