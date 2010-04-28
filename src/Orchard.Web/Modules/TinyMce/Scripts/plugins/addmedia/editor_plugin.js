﻿(function() { tinymce.PluginManager.requireLangPack("addmedia"); tinymce.create('tinymce.plugins.Orchard.AddMedia', { init: function(ed, url) { ed.addCommand('mceAddMedia', function() { ed.windowManager.open({ file: url + '/addmedia.htm', width: 550 + parseInt(ed.getLang('addmedia.delta_width', 0)), height: 110 + parseInt(ed.getLang('addmedia.delta_height', 0)), inline: 1 }, { plugin_url: url }) }); ed.addButton('addmedia', { title: ed.getLang('addmedia.title'), cmd: 'mceAddMedia', image: url + '/img/picture_add.png' }) }, createControl: function(n, cm) { return null }, getInfo: function() { return { longname: 'Orchard AddMedia Plugin', author: 'Nathan Heskew', authorurl: 'http://orchardproject.net', infourl: 'http://orchardproject.net', version: '0.1'} } }); tinymce.PluginManager.add('addmedia', tinymce.plugins.Orchard.AddMedia) })();