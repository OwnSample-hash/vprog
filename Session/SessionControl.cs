using System.Windows;
using System.Windows.Controls;

namespace car.Session {

  public partial class Session : Control {

    public static readonly DependencyProperty TypeProperty =
      DependencyProperty.Register("Type",
        typeof(ESessionType),
        typeof(Session),
        new PropertyMetadata(ESessionType.None, OnTypeChanged)
      );

    public ESessionType Type {
      get { return (ESessionType)GetValue(TypeProperty); }
      set { SetValue(TypeProperty, value); }
    }

    static Session() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(Session), new FrameworkPropertyMetadata(typeof(Session)));
    }

    public override void OnApplyTemplate() {
      base.OnApplyTemplate();
      UpdateVisualState();
      if (GetTemplateChild("btLogin") is Button button) {
        button.Click += Login;
      }
      if (GetTemplateChild("btLogout") is Button button2) {
        button2.Click += Logout;
      }
      if (GetTemplateChild("btAdmin") is Button btAdmin) {
        btAdmin.Click += (_, _) => { new AdminTool.AdminTool().Show(); };
      }
    }

    private static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      if (d is Session session) {
        session.UpdateVisualState();
      }
    }

    private void UpdateVisualState() {
      string state = $"LoggedIn_{User.Permission}";
      if (GetTemplateChild("SessionType") is Label sessionType) {
        if (User.Permission == ESessionType.None)
          sessionType.Content = "Welcome! Please login!";
        else
          sessionType.Content = $"Welcome! {User.Username}";
      }
      if (VisualStateManager.GoToState(this, state, true))
        Console.WriteLine("Switched state");
      else
        Console.WriteLine("failed to switch state");
    }
  }
}
