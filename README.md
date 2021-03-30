# WPFLocalizationExtension
[![CodeFactor](https://www.codefactor.io/repository/github/xamlmarkupextensions/wpflocalizationextension/badge/master)](https://www.codefactor.io/repository/github/xamlmarkupextensions/wpflocalizationextension/overview/master)
[![Nuget](https://img.shields.io/nuget/v/WpfLocalizeExtension.svg)](https://www.nuget.org/packages/WpfLocalizeExtension)
![.NET](https://github.com/XAMLMarkupExtensions/WPFLocalizationExtension/workflows/.NET/badge.svg)
[![Join the chat at https://gitter.im/SeriousM/WPFLocalizationExtension](https://badges.gitter.im/SeriousM/WPFLocalizationExtension.svg)](https://gitter.im/SeriousM/WPFLocalizationExtension?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

#### easiest way to localize any type of DependencyProperties or native Properties on DependencyObjects since 2008!

## Supported Frameworks

* .Net Framework 4.0+
* .Net CORE 3.0+

## Localization Tools
[ResXManager (Visual Studio Plugin and StandAlone)](http://visualstudiogallery.msdn.microsoft.com/3b64e04c-e8de-4b97-8358-06c73a97cc68)  
[Zeta Resource Editor (Freeware)](http://www.zeta-resource-editor.com/index.html)

## Installation

[NuGet Package](https://nuget.org/packages/WpfLocalizeExtension/)

```net
dotnet add package WPFLocalizationExtension
```

## Features

- Design Time Preview
- Binding Support
- Dynamic language change in runtime
- Supports multiple resources
- Supports multiple assemblies
- Interface for custom providers
- Get the list of all available cultures from a provider
- Multiple UI thread support
- Automatic key lookup
- List of available cultures
- Culture swapping by Commands (SetCultureCommand)
- Works with the .resx-fallback mechanism (e.g. en-us -> en -> invariant culture)
- Supports culture forcing (e.g. "this object has to be in english all the time")
- Buffering allows fast switching of the language at runtime
- Can be used with any resource file (.resx) across all assemblies
- Can localize any type of data type, as long a TypeConverter exists for it

## Getting started
Step 1: Add a reference to WpfLocalizeExtension or search for WpfLocalizeExtension on the nuget;

```Install-Package WpfLocalizeExtension```

Step 2: Add NameSpace: ```xmlns:lex="http://wpflocalizeextension.codeplex.com```

### Create resource files

First you need to create a folder, here our folder called `Strings` We are going to create language files in this folder.
add resource files into this folder you can Right-click on folder and from the Add New Item option Select `Resources file` Be careful when naming it.

- The default language can be without a language code
- No matter how many languages you have, you need to create a resource file for that language.
- You must include a 2-digit or 5-digit language code in resource file name.

We want our program to have 2 languages, `English` and `Persian` So I create 2 resource files Pay attention to the naming method.

- `Strings.resx`
- `Strings.en.resx` or `Strings.en-US.resx`


> **_NOTE:_** You can change `Strings` to anything else

### Set up the provider
You have to initialize the localization engine in order to get values at design-time and to set up default dictionaries and assemblies.

#### DefaultDictionary
Specifies the resource file

```xml
lex:ResxLocalizationProvider.DefaultDictionary="Strings"
```

#### DefaultAssembly
Specifies the assembly name

```xml
lex:ResxLocalizationProvider.DefaultAssembly="WpfAppTest"
```

### Design-Time Preview
You can see the text preview in design mode, Just select the desired culture

```xml
lex:LocalizeDictionary.DesignCulture="fa"
```

The value of DesignCulture can be changed dynamically during design time and will have an immediate effect on all localized properties. This gives you the possibility to check the correct localization directly in the WPF designer.

```xml
<Window xmlns:hc="https://handyorg.github.io/handycontrol"
        lex:LocalizeDictionary.DesignCulture="en"
        lex:ResxLocalizationProvider.DefaultAssembly="WpfAppTest"
        lex:ResxLocalizationProvider.DefaultDictionary="Strings">
<!-- Some controls -->
</Window>
```

### Localize

```xml
{lex:Loc Key=, ForceCulture=, Converter=, ConverterParameter=,}
```

|Parameter|Description|
|-|-|
|Key (default)|the key for the resource for the resource lookup|
|ForceCulture|enforce a different culture for this resource lookup|
|Converter|like Converter of Binding Class|
|ConverterParameter|like ConverterParameter of Binding Class|

#### Keys
``` xml
<Button Content="{lex:Loc TextKey}"/>
 <!-- OR -->
<Button Content="{lex:Loc Key=TextKey}"/>
 <!-- OR -->
<TextBlock Text="{lex:Loc {Binding Path=language, FallbackValue=en}}"/>
```

> **_NOTE:_** `TextKey` is the key you created in the resource (resx) file

#### Automatic key retrieval
if the control already got or can get a value for its Name or x:Name property, the key may be neglected, provided, that a key exists that matches one of the following criteria (checked in exactly this order):

- ControlName_PropertyName
- ControlName
Using our button example we could imagine the following scenario:

``` xml
<Button Name="MyButton" Content="{lex:Loc}" />
```
As no key was provided, the extension will first try to resolve a resource key named `MyButton_Content`. If this fails, it will then look for a resource key named `MyButton`. If this fails too, no value will be provided.

The separation character (default is underscore) can be individually set up for each control using the `LocalizeDictionary.Separation` property.

#### Binding

Example 1:
```xml
<Style x:Key="hardCodedStyle1" TargetType="TextBlock">
    <Setter Property="Text" Value="{lex:BLoc Key=TextKey}" />
</Style>
```
Example 2:
```xml
<Style x:Key="hardCodedStyle" TargetType="TextBlock">
    <Setter Property="Text" >
        <Setter.Value>
            <MultiBinding Converter="{lex:TranslateConverter}" >
                <Binding Path="language" />
                <Binding Source="{x:Static lex:LocalizeDictionary.Instance}" Path="Culture"/>
            </MultiBinding>
        </Setter.Value>
    </Setter>
    <Setter Property="Foreground">
        <Setter.Value>
            <MultiBinding Converter="{lex:TranslateConverter}" FallbackValue="#00FF00" >
                <Binding Path="color"/>
                <Binding Source="{x:Static lex:LocalizeDictionary.Instance}" Path="Culture"/>
            </MultiBinding>
        </Setter.Value>
    </Setter>
</Style>
```
Example 3:
```xml
<TextBox>
    <TextBox.Text>
        <MultiBinding StringFormat="Binding test (no design time!): {0} - {1}">
            <MultiBinding.Bindings>
                <lex:BLoc Key="AssemblyTest:CountryRes:Country" />
                <lex:BLoc Key="AssemblyTest:CountryRes:Area" />
            </MultiBinding.Bindings>
        </MultiBinding>
    </TextBox.Text>
</TextBox>
```
Example 4:
```xml
<TextBlock Name="MyLabel3" FontSize="20" HorizontalAlignment="Center">
    <TextBlock.Text>
        <MultiBinding Converter="{lex:StringFormatConverter}" >
            <lex:BLoc Key="HelloWorldWPF:Ressourcen:MyLabel2"/>
            <Binding Path="Hours" FallbackValue=""/>
        </MultiBinding>
    </TextBlock.Text>
</TextBlock>
```
Example 5:
```xml
<TabControl ItemsSource="{Binding Items}">
    <TabControl.ItemTemplate>
        <DataTemplate DataType="{x:Type TabItem}">
            <StackPanel Orientation="Horizontal">
                <StackPanel.Resources>
                    <lex:Loc x:Key="LocalizedHeader" x:Name="LocalizedHeader" />
                </StackPanel.Resources>
                <lex:BLoc Source="{Binding TranslationKey}" Target="{x:Reference LocalizedHeader}" />
                <TextBlock Text="{x:Reference LocalizedHeader}" />
            </StackPanel>
        </DataTemplate>
    </TabControl.ItemTemplate>
</TabControl>
```

### Code-Behind
It is very easy to use in Code-Behind
```CS
var obj = LocalizeDictionary.Instance.GetLocalizedObject("key");
// OR
var text = LocalizeDictionary.Instance.GetLocalizedString("key");
MessageBox.Show(text);
```

### Change language
To change the language in runtime, just write the following code and specify the language code
``` CS
LocalizeDictionary.Instance.Culture = new CultureInfo("fa");
```

or you can change culture in xaml by command
Example 1:
```xml
<Button Content="Change Language" CommandParameter="fa" Command="{Binding Source={x:Static lex:LocalizeDictionary.Instance}, Path=SetCultureCommand}"/>
```
Example 2:
```xml
<ComboBox ItemsSource="{Binding Source={x:Static lex:LocalizeDictionary.Instance}, Path=MergedAvailableCultures}"
            SelectedItem="{Binding Source={x:Static lex:LocalizeDictionary.Instance}, Path=Culture}"
            DisplayMemberPath="NativeName"/>
```

### Fix ContextMenu, DataGrid and Template
You may encounter errors similar to the following error when using the `ContextMenu`, `DataGrid` and `Template` (mybe other controls too)

```
System.Windows.Data Error: 4 : Cannot find source for binding with reference 'RelativeSource FindAncestor, AncestorType='System.Windows.FrameworkElement', AncestorLevel='1''. BindingExpression:Path=Parent; DataItem=null; target element is 'ContextMenu' (Name=''); target property is 'Parent' (type 'DependencyObject')
```

> **_NOTE:_** This is because ContextMenu/DataGrid/Template breaks the Parent/Child relationships towards the host control or window. Therefore, the assembly and dictionary attached properties can't be read out there. 

Solution: Add the `assembly` and `dictionary` to the `key`, according to the: `"Assembly:Dictionary:Key"` notation. Or add these attached properties to a suitable place in visual subtree

```xml
<ContextMenu>
    <MenuItem Header="{lex:Loc Assembly:Dictionary:Key}"/>
</ContextMenu>
```

OR

```xml
<ContextMenu lex:ResxLocalizationProvider.DefaultAssembly="WpfAppTest"
             lex:ResxLocalizationProvider.DefaultDictionary="Strings">
    <MenuItem Header="{lex:Loc Key}"/>
</ContextMenu>
```

### Advanced

#### Multiple-assemblies-and-dictionaries
There may be situations, where more than one assembly and/or dictionary is used for a single XAML code file. This scenario is fully supported by the `LocExtension` and `ResxLocalizationProvider`.

##### Switching assemblies and dictionaries
You may change the values of `ResxLocalizationProvider.DefaultAssembly` and `ResxLocalizationProvider.DefaultDictionary` on arbitrary locations throughout the VisualTree of your XAML code - where and as often you need it. The various LocExtension instances will automatically resolve the corresponding values. This implies, that you may also change both values dynamically during runtime.

##### Directly provided assembly and/or dictionary
You may also define the assembly or dictionary that will be used at each instance of LocExtension by using their properties Assembly or Dict respectively. Alternatively, you may use the constructor syntax already described for the keys. The following three buttons will have the same text:

```xml
<Button Content="{lex:Loc MyAssembly:MyResource:Test}" />
<!-- If you need different Assembly or Dic you should use the attached properties of ResxLocalizationProvider -->
<Button Content="{lex:Loc Test}" />
<Button Content="{lex:Loc Key=Test}" />
```

##### Assembly name differing from the default namespace
This scenario is supported by the ResxLocalizationProvider. In case you have several resource files in a single assembly with the same name but in different namespaces, you have to provide a qualified name of your dictionary.

#### Provider
The project was restructured to separate the target identification and value conversion in the markup extension from the logic that actually provides the value by introducing the IResxLocalizationProvider interface. This enables us to plug in other provider services without touching the base, the LocExtension.

#### Changing the provider
The provider can be changed using the `LocalizeDictionary.Provider` attached property at any node in the XAML document. Depending on the provider, a static singleton reference (e.g. resx provider) must be assigned to this property.

The default provider is set to the `ResxLocalizationProvider`. If you need another application wide default provider, just overwrite the static backed-up attached property `LocalizeDictionary.DefaultProvider`.

#### Provider features
Besides its localized look up functionality, providers can give you a list of available cultures. This observable collection can be read out or bound - whatever you need or prefer. The list is additionally observed by the LocalizeDictionary instance providing a merged bindable list of all available cultures.

Furthermore, providers fire events, when critical values in the provider changed (triggering an update of the LocExtension) or when an error occured.

#### Implementing custom providers
To implement your own provider, create a class that implements the IResxLocalizationProvider interface. There is no restriction concerning its base class.

```cs
public interface IResxLocalizationProvider
{
    /// <summary>
    /// Get the localized object.
    /// </summary>
    /// <param name="key">The key to the value.</param>
    /// <param name="target">The target <see cref="DependencyObject"/>.</param>
    /// <param name="culture">The culture to use.</param>
    /// <returns>The value corresponding to the key and culture.</returns>
    object GetLocalizedObject(string key, DependencyObject target, CultureInfo culture);

    /// <summary>
    /// An observable list of available cultures.
    /// </summary>
    ObservableCollection<CultureInfo> AvailableCultures { get; }

    /// <summary>
    /// An event when the provider changed.
    /// </summary>
    event ProviderChangedEventHandler ProviderChanged;

    /// <summary>
    /// An event when an error occurred.
    /// </summary>
    event ProviderErrorEventHandler ProviderError;
}
```
You can then assign your own provider to the properties of LocalizeDictionary.

#### Value Converters
The library delivers some very usefull ValueConverts. All Valueconverters can be used with the standard syntax and the definition as resource, but the also implement a MarkupExtension declaration so that you can write as an example for the TranslateConverter:

<Element Converter="{lex:TranslateConverter}" />

##### TranslateConverter
This converters allows to make the translation process as a converter. The input of the converter is the key for the lookup and the result the translated result.

##### StringFormatConverter
This converter solves the issue that Binding unfortunately don't support changing of the StringFormat after the firts usage of the Binding. The StringFormatConverter expects as the first value the StringFormat and all further Values are given the StringFormat as paramter. If smartFormat is available it will be automatically used for the StringFormat for extended possibilities especially pluralisation.

##### ToLower & ToUpperConverter
This converter just make an ToLower or ToUpper to the value.

### FAQ
#### Missing Keys
If you like to see what keys are not translated you can easily register to the following event.

```cs
LocalizeDictionary.Instance.MissingKeyEvent += Instance_MissingKeyEvent;
```

The issue or bad design is that this is only triggered if you use the LocExtension, BLoc and not if you make codebehind translation. If you do that you should use a different Event.

```cs
LocalizeDictionary.Instance.GetLocalizedObject(....) 

LocalizeDictionary.Instance.DefaultProvider.ProviderError  += ...
```

#### Binding cannot be changed after it has been used
Binding does not support any changes on Properties after it is bound the firts time. So if you like to do thinks like:

```xml
{Binding Source={lex:Loc Foo}}
{Binding StringFormat={lex:Loc Bar}}
```
This will crash. For StringFormat use cases we have implemented a StringFormatConverter.
