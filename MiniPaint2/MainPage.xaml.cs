using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace MiniPaint2;

public partial class MainPage : ContentPage
{
    public PaintDrawable CanvasDrawable { get; } = new();

    private float _thickness = 5f;
    private Color _currentColor = Colors.Black;
    private bool _isErasing = false;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private void OnThicknessChanged(object sender, ValueChangedEventArgs e)
    {
        _thickness = (float)e.NewValue;
        ThicknessLabel.Text = ((int)_thickness).ToString();
    }

    private void OnUndoClicked(object sender, EventArgs e)
    {
        CanvasDrawable.Undo();
        DrawingCanvas.Invalidate();
    }

    private void OnEraserClicked(object sender, EventArgs e)
    {
        _isErasing = !_isErasing;
        EraserButton.BackgroundColor = _isErasing ? Colors.Orange : Color.FromArgb("#CCCCCC");
    }

    private async void OnClearClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Wyczyść", "Czy na pewno chcesz wyczyścić całe płótno?", "Tak", "Nie");
        if (confirm)
        {
            CanvasDrawable.Clear();
            DrawingCanvas.Invalidate();
        }
    }

    private async void OnColorPickerClicked(object sender, EventArgs e)
    {
        string[] kolory = { "Czarny", "Czerwony", "Zielony", "Niebieski", "Żółty", "Biały", "Fioletowy", "Pomarańczowy" };
        Color[] wartosci = { Colors.Black, Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.White, Colors.Purple, Colors.Orange };

        string wynik = await DisplayActionSheet("Wybierz kolor", "Anuluj", null, kolory);
        int idx = Array.IndexOf(kolory, wynik);
        if (idx >= 0)
        {
            _currentColor = wartosci[idx];
            _isErasing = false;
            EraserButton.BackgroundColor = Color.FromArgb("#CCCCCC");
            ColorPickerButton.BackgroundColor = _currentColor;
        }
    }

    private PointF _lastPoint;

    private void OnCanvasStartInteraction(object sender, TouchEventArgs e)
    {
        _lastPoint = e.Touches[0];
        Color kolor = _isErasing ? Colors.White : _currentColor;
        CanvasDrawable.StartStroke(_lastPoint, kolor, _isErasing ? _thickness * 3 : _thickness);
    }

    private void OnCanvasDragInteraction(object sender, TouchEventArgs e)
    {
        PointF current = e.Touches[0];
        CanvasDrawable.AddPoint(current);
        _lastPoint = current;
        DrawingCanvas.Invalidate();
    }

    private void OnCanvasEndInteraction(object sender, TouchEventArgs e)
    {
        CanvasDrawable.EndStroke();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (!Directory.Exists(folder))
                folder = FileSystem.AppDataDirectory;

            string filename = Path.Combine(folder, $"MiniPaint_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            var screenshot = await Screenshot.CaptureAsync();
            using var stream = await screenshot.OpenReadAsync();
            using var fs = File.Create(filename);
            await stream.CopyToAsync(fs);

            await DisplayAlert("Zapisano", $"Obraz zapisany:\n{filename}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Błąd", $"Nie udało się zapisać:\n{ex.Message}", "OK");
        }
    }

    private async void OnExitClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Zakończ program",
            "Czy na pewno chcesz zakończyć działanie programu?",
            "Tak, zakończ",
            "Nie, wróć");

        if (confirm)
            Application.Current?.Quit();
    }
}