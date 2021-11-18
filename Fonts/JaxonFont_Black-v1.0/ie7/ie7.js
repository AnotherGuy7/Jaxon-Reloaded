/* To avoid CSS expressions while still supporting IE 7 and IE 6, use this script */
/* The script tag referencing this file must be placed before the ending body tag. */

/* Use conditional comments in order to target IE 7 and older:
	<!--[if lt IE 8]><!-->
	<script src="ie7/ie7.js"></script>
	<!--<![endif]-->
*/

(function() {
	function addIcon(el, entity) {
		var html = el.innerHTML;
		el.innerHTML = '<span style="font-family: \'JaxonFont_Black\'">' + entity + '</span>' + html;
	}
	var icons = {
		'icon-0': '&#x30;',
		'icon-1': '&#x31;',
		'icon-2': '&#x32;',
		'icon-3': '&#x33;',
		'icon-4': '&#x34;',
		'icon-5': '&#x35;',
		'icon-6': '&#x37;',
		'icon-7': '&#xe90a;',
		'icon-8': '&#x38;',
		'icon-9': '&#x39;',
		'icon-A': '&#x61;',
		'icon-B': '&#x62;',
		'icon-C': '&#x63;',
		'icon-Colon': '&#x3a;',
		'icon-Comma': '&#x2c;',
		'icon-D': '&#x64;',
		'icon-E': '&#x65;',
		'icon-ExclamMark': '&#x21;',
		'icon-F': '&#x66;',
		'icon-G': '&#x67;',
		'icon-H': '&#x68;',
		'icon-I': '&#x69;',
		'icon-J': '&#x6a;',
		'icon-K': '&#x6b;',
		'icon-L': '&#x6c;',
		'icon-LeftApos': '&#x22;',
		'icon-M': '&#x6d;',
		'icon-N': '&#x6e;',
		'icon-O': '&#x6f;',
		'icon-P': '&#x70;',
		'icon-Period': '&#x2e;',
		'icon-Q': '&#x71;',
		'icon-Question': '&#x3f;',
		'icon-R': '&#x72;',
		'icon-S': '&#x73;',
		'icon-T': '&#x74;',
		'icon-U': '&#x75;',
		'icon-V': '&#x76;',
		'icon-W': '&#x77;',
		'icon-X': '&#x78;',
		'icon-Y': '&#x79;',
		'icon-Z': '&#x7a;',
		'0': 0
		},
		els = document.getElementsByTagName('*'),
		i, c, el;
	for (i = 0; ; i += 1) {
		el = els[i];
		if(!el) {
			break;
		}
		c = el.className;
		c = c.match(/icon-[^\s'"]+/);
		if (c && icons[c[0]]) {
			addIcon(el, icons[c[0]]);
		}
	}
}());
