XTemplate = function(html) {
    var a = arguments;
    if ($.isArray(html)) {
        html = html.join("");
    } else if (a.length > 1) {
        var buf = [];
        for (var i = 0, len = a.length; i < len; i++) {
            if (typeof a[i] == 'object') {
                Ext.apply(this, a[i]);
            } else {
                buf[buf.length] = a[i];
            }
        }
        html = buf.join('');
    }
    this.html = html;
    if (this.compiled) {
        this.compile();
    }

    var s = this.html;

    s = ['<tpl>', s, '</tpl>'].join('');

    var re = /<tpl\b[^>]*>((?:(?=([^<]+))\2|<(?!tpl\b[^>]*>))*?)<\/tpl>/;

    var nameRe = /^<tpl\b[^>]*?for="(.*?)"/;
    var ifRe = /^<tpl\b[^>]*?if="(.*?)"/;
    var execRe = /^<tpl\b[^>]*?exec="(.*?)"/;
    var m, id = 0;
    var tpls = [];
    while (m = s.match(re)) {
        var m2 = m[0].match(nameRe);
        var m3 = m[0].match(ifRe);
        var m4 = m[0].match(execRe);
        var exp = null, fn = null, exec = null;
        var name = m2 && m2[1] ? m2[1] : '';
        if (m3) {
            exp = m3 && m3[1] ? m3[1] : null;
            if (exp) {
                fn = new Function('values', 'parent', 'xindex', 'xcount', 'with(values){ return ' + (Format.htmlDecode(exp)) + '; }');
            }
        }
        if (m4) {
            exp = m4 && m4[1] ? m4[1] : null;
            if (exp) {
                exec = new Function('values', 'parent', 'xindex', 'xcount', 'with(values){ ' + (Format.htmlDecode(exp)) + '; }');
            }
        }
        if (name) {
            switch (name) {
                case '.': name = new Function('values', 'parent', 'with(values){ return values; }'); break;
                case '..': name = new Function('values', 'parent', 'with(values){ return parent; }'); break;
                default: name = new Function('values', 'parent', 'with(values){ return ' + name + '; }');
            }
        }
        tpls.push({
            id: id,
            target: name,
            exec: exec,
            test: fn,
            body: m[1] || ''
        });
        s = s.replace(m[0], '{xtpl' + id + '}');
        ++id;
    }
    for (var i = tpls.length - 1; i >= 0; --i) {
        this.compileTpl(tpls[i]);
    }
    this.master = tpls[tpls.length - 1];
    this.tpls = tpls;
    this.codeRe = /\{\[((?:\\\]|.|\n)*?)\]\}/g;
    this.re = /\{([\w-\.\#]+)(?:\:([\w\.]*)(?:\((.*?)?\))?)?(\s?[\+\-\*\\]\s?[\d\.\+\-\*\\\(\)]+)?\}/g;
}
XTemplate.prototype.re = /\{([\w-\.\#]+)(?:\:([\w\.]*)(?:\((.*?)?\))?)?(\s?[\+\-\*\\]\s?[\d\.\+\-\*\\\(\)]+)?\}/g;
XTemplate.prototype.codeRe = /\{\[((?:\\\]|.|\n)*?)\]\}/g;
XTemplate.prototype.disableFormats = false;
XTemplate.prototype.append = function(el, values, returnElement) {
    return this.doInsert('beforeEnd', el, values, returnElement);
};
XTemplate.prototype.apply = function() { };
XTemplate.prototype.applySubTemplate = function(id, values, parent, xindex, xcount) {
    var t = this.tpls[id];
    if (t.test && !t.test.call(this, values, parent, xindex, xcount)) {
        return '';
    }
    if (t.exec && t.exec.call(this, values, parent, xindex, xcount)) {
        return '';
    }
    var vs = t.target ? t.target.call(this, values, parent) : values;
    parent = t.target ? values : parent;
    if (t.target && $.isArray(vs)) {
        var buf = [];
        for (var i = 0, len = vs.length; i < len; i++) {
            buf[buf.length] = t.compiled.call(this, vs[i], parent, i + 1, len);
        }
        return buf.join('');
    }
    return t.compiled.call(this, vs, parent, xindex, xcount);
};
XTemplate.prototype.applyTemplate = function(values) {
    return this.master.compiled.call(this, values, {}, 1, 1);
};
XTemplate.prototype.call = function(fnName, value, allValues) {
    return this[fnName](value, allValues);
};
XTemplate.prototype.compile = function() {
    return this;
};
XTemplate.prototype.compileTpl = function(tpl) {
    var fm = Format;
    var useF = this.disableFormats !== true;
    var ua = navigator.userAgent.toLowerCase();
    isChrome = ua.indexOf("chrome") > -1;
    isSafari = !isChrome && (/webkit|khtml/).test(ua);
    isGecko = !isSafari && !isChrome && ua.indexOf("gecko") > -1;
    var sep = isGecko ? "+" : ",";
    var fn = function(m, name, format, args, math) {
        if (name.substr(0, 4) == 'xtpl') {
            return "'" + sep + 'this.applySubTemplate(' + name.substr(4) + ', values, parent, xindex, xcount)' + sep + "'";
        }
        var v;
        if (name === '.') {
            v = 'values';
        } else if (name === '#') {
            v = 'xindex';
        } else if (name.indexOf('.') != -1) {
            v = name;
        } else {
            v = "values['" + name + "']";
        }
        if (math) {
            v = '(' + v + math + ')';
        }
        if (format && useF) {
            args = args ? ',' + args : "";
            if (format.substr(0, 5) != "this.") {
                format = "fm." + format + '(';
            } else {
                format = 'this.call("' + format.substr(5) + '", ';
                args = ", values";
            }
        } else {
            args = ''; format = "(" + v + " === undefined ? '' : ";
        }
        return "'" + sep + format + v + args + ")" + sep + "'";
    };
    var codeFn = function(m, code) {
        return "'" + sep + '(' + code + ')' + sep + "'";
    };

    var body;
    // branched to use + in gecko and [].join() in others
    if (isGecko) {
        body = "tpl.compiled = function(values, parent, xindex, xcount){ return '" +
                   tpl.body.replace(/(\r\n|\n)/g, '\\n').replace(/'/g, "\\'").replace(this.re, fn).replace(this.codeRe, codeFn) +
                    "';};";
    } else {
        body = ["tpl.compiled = function(values, parent, xindex, xcount){ return ['"];
        body.push(tpl.body.replace(/(\r\n|\n)/g, '\\n').replace(/'/g, "\\'").replace(this.re, fn).replace(this.codeRe, codeFn));
        body.push("'].join('');};");
        body = body.join('');
    }
    eval(body);
    return this;
};
XTemplate.prototype.doInsert = function(where, el, values, returnEl) {
    el = Ext.getDom(el);
    var newNode = Ext.DomHelper.insertHtml(where, el, this.applyTemplate(values));
    return returnEl ? Ext.get(newNode, true) : newNode;
};
XTemplate.prototype.insertAfter = function(el, values, returnElement) {
    return this.doInsert('afterEnd', el, values, returnElement);
};
XTemplate.prototype.insertBefore = function(el, values, returnElement) {
    return this.doInsert('beforeBegin', el, values, returnElement);
};
XTemplate.prototype.insertFirst = function(el, values, returnElement) {
    return this.doInsert('afterBegin', el, values, returnElement);
};
XTemplate.prototype.override = function() { };
XTemplate.prototype.overwrite = function(el, values) {
    if (el.jquery == null) {
        el = $("#" + el);
    }
    el.html(this.applyTemplate(values));
    return el;
};
XTemplate.prototype.set = function(html, compile) {
    this.html = html;
    this.compiled = null;
    if (compile) {
        this.compile();
    }
    return this;
};
XTemplate.from = function(el, config) {
    el = XTemplate.getDom(el);
    return new XTemplate(el.value || el.innerHTML);
}
XTemplate.getDom = function(el) {
    if (!el || !document) {
        return null;
    }
    return el.dom ? el.dom : (typeof el == 'string' ? document.getElementById(el) : el);
};