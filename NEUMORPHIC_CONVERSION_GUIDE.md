# 📘 Przewodnik konwersji okien na styl neumorficzny

## 🎯 Wzorzec konwersji dla pozostałych okien

Aby skonwertować okno do stylu neumorficznego, wykonaj następujące kroki:

---

## 1️⃣ KROK 1: Właściwości Window

```xml
<!-- PRZED -->
<Window Background="#1E1E1E" Foreground="White" WindowStyle="ToolWindow">

<!-- PO -->
<Window Background="Transparent"
        Foreground="{StaticResource NeumorphicTextBrush}"
        WindowStyle="None"
        AllowsTransparency="True">
```

---

## 2️⃣ KROK 2: Resources - Import stylów

```xml
<Window.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="/PrettyScreenSHOT;component/NeumorphicStyles.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Window.Resources>
```

**USUŃ** wszystkie lokalne style (TextBlock, Button, ComboBox itp.)

---

## 3️⃣ KROK 3: Główna struktura z chrome

```xml
<!-- Neumorphic window chrome -->
<Border Style="{StaticResource NeumorphicPanel}" CornerRadius="18" Margin="15" Padding="0">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="48"/> <!-- Title bar -->
            <RowDefinition Height="*"/>  <!-- Main content -->
        </Grid.RowDefinitions>

        <!-- Custom Title Bar -->
        <Border Grid.Row="0" Background="Transparent" Padding="15,10" CornerRadius="18,18,0,0"
                MouseLeftButtonDown="OnTitleBarMouseDown">
            <DockPanel>
                <TextBlock Text="Tytuł okna" FontSize="20"
                           Style="{StaticResource NeumorphicHeading}"
                           VerticalAlignment="Center" DockPanel.Dock="Left"/>

                <Button Click="OnCloseClick" Style="{StaticResource NeumorphicCircularButton}"
                        Width="32" Height="32" DockPanel.Dock="Right"
                        HorizontalAlignment="Right" ToolTip="Zamknij">
                    <TextBlock Text="✕" FontSize="16"/>
                </Button>
            </DockPanel>
        </Border>

        <!-- Main Content -->
        <Grid Grid.Row="1" Margin="20,10,20,20">
            <!-- Twoja zawartość tutaj -->
        </Grid>
    </Grid>
</Border>
```

---

## 4️⃣ KROK 4: Zamień kontrolki na neumorficzne

### **TextBlock**
```xml
<!-- PRZED -->
<TextBlock Text="Label:" FontWeight="Bold" Foreground="White"/>

<!-- PO -->
<TextBlock Text="Label:" Style="{StaticResource NeumorphicTextBlock}" FontWeight="Regular"/>
```

### **TextBox**
```xml
<!-- PRZED -->
<TextBox x:Name="MyTextBox" Background="#2D2D2D" Foreground="White"/>

<!-- PO -->
<TextBox x:Name="MyTextBox" Style="{StaticResource NeumorphicTextBox}"/>
```

### **ComboBox**
```xml
<!-- PRZED -->
<ComboBox x:Name="MyCombo" Background="#2D2D2D"/>

<!-- PO -->
<ComboBox x:Name="MyCombo" Style="{StaticResource NeumorphicComboBox}"/>
```

### **CheckBox**
```xml
<!-- PRZED -->
<CheckBox Content="Option" Foreground="White"/>

<!-- PO -->
<CheckBox Content="Option" Style="{StaticResource NeumorphicCheckBox}"/>
```

### **Slider**
```xml
<!-- PRZED -->
<Slider Minimum="0" Maximum="100"/>

<!-- PO -->
<Slider Minimum="0" Maximum="100" Style="{StaticResource NeumorphicSlider}"/>
```

### **Button - standardowy**
```xml
<!-- PRZED -->
<Button Content="OK" Background="#2196F3" Foreground="White"/>

<!-- PO - monochromatyczny! -->
<Button Content="OK" Style="{StaticResource NeumorphicRaisedButton}"/>
```

### **Button - główna akcja (Save, OK)**
```xml
<!-- Użyj NeumorphicDepressedButton dla głównej akcji -->
<Button Content="Save" Style="{StaticResource NeumorphicDepressedButton}"/>
```

### **Button - okrągły (zamknij, ikony)**
```xml
<Button Style="{StaticResource NeumorphicCircularButton}" Width="36" Height="36">
    <TextBlock Text="✕" FontSize="14"/>
</Button>
```

---

## 5️⃣ KROK 5: Usuń wszystkie kolorowe elementy

### ❌ USUŃ te kolory:
- `Background="#2196F3"` (niebieski)
- `Background="#F44336"` (czerwony)
- `Background="#4CAF50"` (zielony)
- `Background="#FF9800"` (pomarańczowy)
- `Background="#60A5FA"` (jasny niebieski)
- `Background="#1E1E1E"` (ciemny)
- `Background="#2D2D2D"` (ciemny szary)
- `BorderBrush="#404040"` (ciemny border)
- `Foreground="#CCCCCC"` (szary tekst)
- `Foreground="White"` (biały tekst)

### ✅ ZAMIEŃ na:
- Używaj tylko `{StaticResource NeumorphicXxxBrush}`
- Przyciski bez Background (używają stylu)
- Tylko monochromatyczne kolory (#F0F0F0, #BEBEBE, #212121)

---

## 6️⃣ KROK 6: Hierarchia wizualna bez kolorów

**Jak wyróżnić elementy bez kolorów:**

| Element | Styl |
|---------|------|
| Główna akcja (Save, OK) | `NeumorphicDepressedButton` + FontWeight="Bold" |
| Akcje drugorzędne | `NeumorphicRaisedButton` |
| Tekst nagłówka | `NeumorphicHeading` (20-22px) |
| Tekst normalny | `NeumorphicTextBlock` (14px) |
| Tekst drugorzędny | `NeumorphicTextBlockSecondary` (12px) |
| Panele | `NeumorphicPanel` (wypukły z cieniem) |

---

## 7️⃣ KROK 7: Code-behind - Drag window handler

Dodaj do pliku `.cs`:

```csharp
private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
{
    if (e.ChangedButton == MouseButton.Left)
    {
        this.DragMove();
    }
}
```

---

## 📋 CHECKLIST konwersji

Dla każdego okna sprawdź:

- [ ] Window: Background="Transparent", WindowStyle="None", AllowsTransparency="True"
- [ ] Window.Resources: Import NeumorphicStyles.xaml
- [ ] Główny Border z NeumorphicPanel, CornerRadius="18", Margin="15"
- [ ] Custom title bar z NeumorphicCircularButton do zamykania
- [ ] Wszystkie TextBlock używają NeumorphicTextBlock lub NeumorphicHeading
- [ ] Wszystkie TextBox używają NeumorphicTextBox
- [ ] Wszystkie ComboBox używają NeumorphicComboBox
- [ ] Wszystkie CheckBox używają NeumorphicCheckBox
- [ ] Wszystkie Slider używają NeumorphicSlider
- [ ] Wszystkie Button używają NeumorphicRaisedButton lub NeumorphicDepressedButton
- [ ] BRAK hardcoded kolorów (#2196F3, #F44336, #4CAF50 itp.)
- [ ] BRAK Foreground="White" (tylko {StaticResource NeumorphicTextBrush})
- [ ] Event handler OnTitleBarMouseDown w code-behind

---

## 🎨 Pozostałe okna do konwersji

1. **ScreenshotHistoryWindow.xaml** - konwersja z ciemnego motywu
2. **UpdateWindow.xaml** - usuń niebieski akcent #60A5FA
3. **UpdateProgressWindow.xaml**
4. **UpdateNotificationWindow.xaml**
5. **SaveScreenshotDialog.xaml**
6. **VideoCaptureWindow.xaml**
7. **ScreenshotOverlay.xaml** - może wymagać specjalnego traktowania (overlay)

---

## 💡 Wskazówki

- **Separator**: Jeśli używasz, zmień `Background="#D0D0D0"` na `{StaticResource NeumorphicDarkShadowBrush}` i Opacity="0.3"
- **Border**: Używaj `NeumorphicPanel` dla wypukłych paneli lub `NeumorphicDepressedBorder` dla wklęsłych
- **ScrollViewer**: Dodaj `Style="{StaticResource NeumorphicScrollBar}"` jeśli chcesz neumorficzny scrollbar
- **Padding/Margin**: Duże odstępy (15-20px) są kluczowe dla neumorfizmu!

Powodzenia! 🚀
