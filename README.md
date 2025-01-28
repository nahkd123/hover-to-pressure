# Hover to pressure
This OTD plugin converts hover distance reported from tablets to pen pressure.

This plugin only works for tablets that reports hover distance, and it seems like only Wacom tablets report this.

## Usage
### Installing the plugin
1. Open Releases page (or [click here](https://github.com/nahkd123/hover-to-pressure/releases) to open).
1. Download nahkd123.HoverToDistance.zip.
1. Drag that zip file to Plugin Manager to install (Plugins > Open Plugin Manager...).
1. You can now enable this plugin in Filters tab.

### Configuring the plugin
- Minimum/Maximum hover distance: The hover distance range that will be mapped to pen pressure. Check hover distance in Tablet Debugger window.
- Activation mode: How the plugin should be activated.
	+ Always: Always activate (when the plugin is enabled, of course);
	+ PenButton: Only activate if certain pen button is pressed (airbrush-like experience);
	+ AuxButton: Only activate if certain auxillary button is pressed.
- Activation button index: The index of button that will be used to activate plugin, starting from 1. Only works if activation mode is PenButton or AuxButton.
- Smoothing weight: The amount of smoothing weight to apply on hover distance detected by this plugin. The smoothing formula is `(previous * weight + next) / (weight + 1)`.

## License
MIT license.