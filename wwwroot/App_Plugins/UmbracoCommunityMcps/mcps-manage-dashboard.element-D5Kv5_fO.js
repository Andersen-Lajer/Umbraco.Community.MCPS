import { UmbLitElement as v } from "@umbraco-cms/backoffice/lit-element";
import { html as d, css as b, state as n, query as w, customElement as D } from "@umbraco-cms/backoffice/external/lit";
import { UMB_CURRENT_USER_CONTEXT as T } from "@umbraco-cms/backoffice/current-user";
import { UmbDataTypeDetailRepository as P, UMB_DATA_TYPE_PICKER_MODAL as C } from "@umbraco-cms/backoffice/data-type";
import { UMB_MODAL_MANAGER_CONTEXT as S } from "@umbraco-cms/backoffice/modal";
var c = /* @__PURE__ */ ((t) => (t.Relevant = "Relevant", t.Newest = "Newest", t.MostPopular = "MostPopular", t))(c || {}), u = /* @__PURE__ */ ((t) => (t.UniquePage = "UniquePage", t.AllowDuplicates = "AllowDuplicates", t))(u || {});
class k {
  constructor(e) {
    this.id = e.id, this.name = e.name, this.propagationPriority = e.propagationPriority ?? [
      c.Relevant,
      c.Newest,
      c.MostPopular
    ], this.duplicationStrategy = e.duplicationStrategy ?? u.UniquePage, this.contentTypes = e.contentTypes, this.propertyAlias = e.propertyAlias, this.fallbackSetting = e.fallbackSetting;
  }
}
var x = Object.defineProperty, M = Object.getOwnPropertyDescriptor, g = (t) => {
  throw TypeError(t);
}, r = (t, e, o, s) => {
  for (var i = s > 1 ? void 0 : s ? M(e, o) : e, p = t.length - 1, h; p >= 0; p--)
    (h = t[p]) && (i = (s ? h(e, o, i) : h(i)) || i);
  return s && i && x(e, o, i), i;
}, f = (t, e, o) => e.has(t) || g("Cannot " + o), m = (t, e, o) => (f(t, e, "read from private field"), e.get(t)), _ = (t, e, o) => e.has(t) ? g("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, o), E = (t, e, o, s) => (f(t, e, "write to private field"), e.set(t, o), o), l, y;
let a = class extends v {
  constructor() {
    super(), _(this, l), _(this, y, new P(this)), this._filteredDocumentTypes = [], this._selectedDocTypes = [], this._toastMessage = "", this._toastColor = "positive", this._toastOpen = !1, this._createDatatype = (t) => {
      t == null || t.preventDefault();
      const e = this._validateInputs();
      if (e) {
        this._showToast(e, "danger");
        return;
      }
      try {
        const o = new k({
          name: this._selectedDatatype ?? "Unnamed",
          propagationPriority: [this.mapPriority(this._priority ?? "")],
          duplicationStrategy: this.mapDuplicationStrategy(this._duplicationStrategy ?? ""),
          contentTypes: this._selectedDocTypes,
          propertyAlias: this._selectedDatatype ?? "unknown"
        }), s = `/mcps/api/rules/create/${this._userEmail}`;
        fetch(s, {
          method: "POST",
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify(o)
        }).then(async (i) => {
          const p = await i.text();
          if (!i.ok)
            throw this._showToast("Datatype creation failed!", "danger"), new Error(`Server error: ${i.status} - ${p}`);
          this._showToast("Datatype created successfully!", "positive");
        }).catch((i) => {
          this._showToast(`Error creating datatype: ${i.message}`, "danger");
        });
      } catch (o) {
        console.error("Unexpected error in _createDatatype:", o);
      }
    }, this.consumeContext(T, (t) => {
      this._observeCurrentUser(t);
    }), this.consumeContext(S, (t) => {
      E(this, l, t);
    });
  }
  connectedCallback() {
    super.connectedCallback(), this._fetchDocumentTypes();
  }
  async _observeCurrentUser(t) {
    this.observe(t.currentUser, (e) => {
      this._currentUser = e, this._userEmail = (e == null ? void 0 : e.email) ?? "unknown";
    });
  }
  async _fetchDocumentTypes() {
    try {
      const t = await fetch("/mcps/api/documenttypes/all");
      if (t.ok) {
        const e = await t.json();
        this._filteredDocumentTypes = e.filter((o) => {
          var s;
          return (s = o.alias) == null ? void 0 : s.toLowerCase().startsWith("mcps");
        }).map((o) => {
          var s;
          return {
            name: o.name,
            alias: o.alias,
            propertyTypes: ((s = o.propertyTypes) == null ? void 0 : s.map((i) => ({
              name: i.name
            }))) ?? []
          };
        });
      } else
        console.error("Failed to fetch document types:", t.statusText);
    } catch (t) {
      console.error("Error fetching document types:", t);
    }
  }
  _onPriorityChange(t) {
    const e = t.target;
    this._priority = e.value;
  }
  _onDuplicationStrategyChange(t) {
    const e = t.target;
    this._duplicationStrategy = e.value;
  }
  _onDocTypePickerInput(t) {
    const e = t.target, o = e.value;
    e && e.value && (this._selectedDocTypes = o.split(","));
  }
  mapPriority(t) {
    switch (t) {
      case "MostPopular":
        return c.MostPopular;
      case "Relevant":
        return c.Relevant;
      case "Newest":
        return c.Newest;
      default:
        return c.Relevant;
    }
  }
  mapDuplicationStrategy(t) {
    switch (t) {
      case "UniquePage":
        return u.UniquePage;
      default:
        return u.UniquePage;
    }
  }
  _validateInputs() {
    const t = [];
    return this._selectedDatatype || t.push("Datatype"), this._priority || t.push("Priority"), this._duplicationStrategy || t.push("Duplication Strategy"), this._selectedDocTypes.length || t.push("Document Types"), t.length > 0 ? `Please select: ${t.join(", ")}` : null;
  }
  _openModal() {
    var e;
    const t = (e = m(this, l)) == null ? void 0 : e.open(this, C, {
      modal: {
        type: "sidebar",
        size: "small"
      }
    });
    t == null || t.onSubmit().then((o) => {
      this._dataTypeId = o.selection[0].toString();
      var s = o.selection[0].toString();
      m(this, y).requestByUnique(s).then((i) => {
        var p;
        this._selectedDatatype = (p = i.data) == null ? void 0 : p.name;
      }).catch(() => {
      });
    }).catch(() => {
    });
  }
  _showToast(t, e) {
    this._toastMessage = t, this._toastColor = e, this._toastOpen = !0, setTimeout(() => {
      this._toastOpen = !1;
    }, 4e3);
  }
  render() {
    var t;
    return d`

<h1>Welcome ${((t = this._currentUser) == null ? void 0 : t.name) ?? "Unknown"}!</h1>
</uui-modal-container>
            <uui-box class="section">
                <p>This is the management dashboard. From here you can manage propagation of content and create new propagation blocks.</p>
                <p>Modular Content Propagation System</p>
            </uui-box>
              
            <div class="twoColumnLayout section">
                <uui-box class="halfWidth">
                    <h2>Create new propagation blocks</h2>
                    <div>
                    <b>Pick document type to propagate</b>
                    <umb-property-editor-ui-document-type-picker @change=${this._onDocTypePickerInput}>
                    <umb-input-document-type>
                    <uui-ref-list>
                    </uui-ref-list>
                    <uui-button>
                    </uui-button>
                    </umb-input-document-type>
                    </umb-property-editor-ui-document-type-picker>
                    <div class="column mt2">
                    <b>Pick datatype to target content</b>
                    <uui-button look="primary" label="Add data type" @click=${this._openModal}></uui-button>
                    <div>Selected Datatype: ${this._selectedDatatype}</div>
                    </div>
                    <div class="dropdowns mt2">
                    <div>
                    <b>Pick by which priority the content should be picked</b>
  <select @change=${this._onPriorityChange}>
  <option value="">-- Select Priority --</option>
  <option value="Relevant">Relevant</option>
  <option value="Newest">Newest</option>
  <option value="MostPopular">MostPopular</option>
</select>
</div>
<div>
<b>Pick the strategy for how duplicates of content are shown on the site</b>
<select @change=${this._onDuplicationStrategyChange}>
  <option value="">-- Select Strategy --</option>
  <option value="UniquePage">UniquePage</option>
 
</select>
 </div>
</div>
<div class="column mt2">
                    <uui-button look="primary" label="Create datatype" @click=${(e) => this._createDatatype(e)}></uui-button>
</div>
                    </div>
                    <div>
                        <umb-property-editor-ui-tags>
                            <umb-tags-input>
                            <div class="wrapper">
                            </div>
                            </umb-tags-input>
                        </umb-property-editor-ui-tags>
                    </div>
                </uui-box>
                <uui-box class="halfWidth">
                    <h2>Current propagation blocks</h2>
                    <ul>
                    ${this._filteredDocumentTypes.map(
      (e) => d`
                            <li>
                                <uui-box headline="${e.alias}" headline-variant="h5">
                                    <p>${e.name}</p>
                                    <ul>
                                        ${e.propertyTypes.map(
        (o) => d`<li>${o.name}</li>`
      )}
                                    </ul>
                                </uui-box>
                            </li>
                        `
    )}
                </ul>
                </uui-box>

               <div class="toastification">
  <uui-toast-notification
    ?open=${this._toastOpen}
    color=${this._toastColor}>
    <uui-toast-notification-layout headline="">
      ${this._toastMessage}
    </uui-toast-notification-layout>
  </uui-toast-notification>
</div>


            </div>
        `;
  }
};
l = /* @__PURE__ */ new WeakMap();
y = /* @__PURE__ */ new WeakMap();
a.styles = [
  b`
            :host {
                display: block;
                padding: 24px;
            }

            .section {
                margin-bottom: 24px;
            }

            .halfWidth {
                width: 49%;
            }

            .twoColumnLayout {
                display: flex;
                justify-content: space-between;
            }

            .column{
                display: flex;
                flex-direction: column;
            }

            .dropdowns{
                display: flex;
                justify-content: space-around;
            }
            
            .mt2{
                margin-top: 1rem;
            }

            .toastification {
    position: fixed;
    bottom: 20px;
    right: 20px;
    z-index: 1000;
    display: flex;
    align-items: center;
    justify-content: center;
}

        `
];
r([
  n()
], a.prototype, "_currentUser", 2);
r([
  n()
], a.prototype, "_filteredDocumentTypes", 2);
r([
  n()
], a.prototype, "_selectedDatatype", 2);
r([
  n()
], a.prototype, "_selectedDocTypes", 2);
r([
  n()
], a.prototype, "_dataTypeId", 2);
r([
  n()
], a.prototype, "_userEmail", 2);
r([
  n()
], a.prototype, "_priority", 2);
r([
  n()
], a.prototype, "_duplicationStrategy", 2);
r([
  n()
], a.prototype, "_toastMessage", 2);
r([
  n()
], a.prototype, "_toastColor", 2);
r([
  n()
], a.prototype, "_toastOpen", 2);
r([
  w("umb-property-editor-ui-document-type-picker")
], a.prototype, "_docTypePickerElement", 2);
a = r([
  D("mcps-manage-dashboard-element")
], a);
const N = a;
export {
  a as McpsManageDashboardElement,
  N as default
};
//# sourceMappingURL=mcps-manage-dashboard.element-D5Kv5_fO.js.map
