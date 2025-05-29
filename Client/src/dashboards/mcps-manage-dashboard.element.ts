import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { html, css, customElement, state } from "@umbraco-cms/backoffice/external/lit";
import { type UmbCurrentUserModel, UMB_CURRENT_USER_CONTEXT } from "@umbraco-cms/backoffice/current-user";
import { UmbDataTypeDetailRepository } from "@umbraco-cms/backoffice/data-type"
import { PropagationSetting } from "../models/propagationSetting"
import { DuplicationStrategy, PropagationRank } from '../models/enums';
import { query } from "@umbraco-cms/backoffice/external/lit";


import {
    UMB_MODAL_MANAGER_CONTEXT
} from "@umbraco-cms/backoffice/modal";
import {
    UMB_DATA_TYPE_PICKER_MODAL
} from "@umbraco-cms/backoffice/data-type";


@customElement('mcps-manage-dashboard-element')
export class McpsManageDashboardElement extends UmbLitElement {
    #modalManagerContext?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

    #datatypeRepository = new UmbDataTypeDetailRepository(this);

    @state()
    private _currentUser?: UmbCurrentUserModel;

    @state()
    private _filteredDocumentTypes: Array<{ name: string; alias: string, propertyTypes: Array<{ name: string }>; }> = [];

    @state()
    private _selectedDatatype?: string;

    @state()
    private _selectedDocTypes: string[] = [];

    @state()
    private _dataTypeId?: string;
    @state()
    private _userEmail?: string;

    @state()
    private _priority?: string;

    @state()
    private _duplicationStrategy?: string;

    @state()
    private _toastMessage: string = '';

    @state()
    private _toastColor: 'positive' | 'danger' = 'positive';

    @state()
    private _toastOpen: boolean = false;


    @query('umb-property-editor-ui-document-type-picker')
    private _docTypePickerElement!: UmbLitElement & { value: any };

    constructor() {
        super();
        this.consumeContext(UMB_CURRENT_USER_CONTEXT, (instance) => {
            this._observeCurrentUser(instance);
        });
        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this.#modalManagerContext = instance;
        });
    }

    connectedCallback(): void {
        super.connectedCallback();
        this._fetchDocumentTypes();
    }

    private async _observeCurrentUser(instance: typeof UMB_CURRENT_USER_CONTEXT.TYPE) {
        this.observe(instance.currentUser, (currentUser) => {
            this._currentUser = currentUser;
            this._userEmail = currentUser?.email ?? 'unknown';
        });
    }

    private async _fetchDocumentTypes() {
        try {
            const response = await fetch('/mcps/api/documenttypes/all');
            if (response.ok) {
                const documentTypes = await response.json();
                // Filter document types where the alias starts with "mcps"
                this._filteredDocumentTypes = documentTypes
                    .filter((doc: any) => doc.alias?.toLowerCase().startsWith("mcps"))
                    .map((doc: any) => ({
                        name: doc.name,
                        alias: doc.alias,
                        propertyTypes: doc.propertyTypes?.map((prop: any) => ({
                            name: prop.name
                        })) ?? [],
                    }));
            } else {
                console.error('Failed to fetch document types:', response.statusText);
            }
        } catch (error) {
            console.error('Error fetching document types:', error);
        }
    }

    private _onPriorityChange(event: Event) {
        const target = event.target as HTMLSelectElement;
        this._priority = target.value;
    }

    private _onDuplicationStrategyChange(event: Event) {
        const target = event.target as HTMLSelectElement;
        this._duplicationStrategy = target.value;
    }

    private _onDocTypePickerInput(event: Event) {

        const picker = event.target as HTMLElement & { value: any };
        const pickerString = picker.value as string;

        if (picker && picker.value) {
            this._selectedDocTypes = pickerString.split(",");
        }
    }

    private mapPriority(priority: string): PropagationRank {
        switch (priority) {
            case 'MostPopular':
                return PropagationRank.MostPopular;
            case 'Relevant':
                return PropagationRank.Relevant;
            case 'Newest':
                return PropagationRank.Newest;
            default:
                return PropagationRank.Relevant;
        }
    }

    private mapDuplicationStrategy(strategy: string): DuplicationStrategy {
        switch (strategy) {
            case 'UniquePage':
                return DuplicationStrategy.UniquePage;
            default:
                return DuplicationStrategy.UniquePage;
        }
    }

    private _validateInputs(): string | null {
        const missingFields: string[] = [];

        if (!this._selectedDatatype) {
            missingFields.push('Datatype');
        }
        if (!this._priority) {
            missingFields.push('Priority');
        }
        if (!this._duplicationStrategy) {
            missingFields.push('Duplication Strategy');
        }
        if (!this._selectedDocTypes.length) {
            missingFields.push('Document Types');
        }

        if (missingFields.length > 0) {
            return `Please select: ${missingFields.join(', ')}`;
        }

        return null;
    }


    private _openModal() {
        const modalContext = this.#modalManagerContext?.open(this, UMB_DATA_TYPE_PICKER_MODAL, {
            modal: {
                type: 'sidebar',
                size: 'small',
            },
        });
        modalContext
            ?.onSubmit()
            .then((value) => {
                this._dataTypeId = value.selection[0]!.toString();
                var dt = value.selection[0]!.toString();
                this.#datatypeRepository.requestByUnique(dt)
                    .then((value2) => {
                        this._selectedDatatype = value2.data?.name
                    })
                    .catch(() => undefined)
            })
            .catch(() => undefined);
    }

    private _showToast(message: string, color: 'positive' | 'danger') {
        this._toastMessage = message;
        this._toastColor = color;
        this._toastOpen = true;

        setTimeout(() => {
            this._toastOpen = false;
        }, 4000); // 4 seconds auto close
    }



    private _createDatatype = (event?: Event) => {
        event?.preventDefault();

        const validationError = this._validateInputs();
        if (validationError) {
            this._showToast(validationError, 'danger');
            return;
        }

        try {
            const propagationSetting = new PropagationSetting({
                name: this._selectedDatatype ?? 'Unnamed',
                propagationPriority: [this.mapPriority(this._priority ?? '')],
                duplicationStrategy: this.mapDuplicationStrategy(this._duplicationStrategy ?? ''),
                contentTypes: this._selectedDocTypes,
                propertyAlias: this._selectedDatatype ?? 'unknown',
            });

            const apiUrl = `/mcps/api/rules/create/${this._userEmail}`;

            fetch(apiUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(propagationSetting),
            })
                .then(async (res) => {
                    const text = await res.text();
                    if (!res.ok) {
                        this._showToast('Datatype creation failed!', 'danger')
                        throw new Error(`Server error: ${res.status} - ${text}`);
                    }
                    this._showToast('Datatype created successfully!', 'positive');
                })
                .catch((error) => {
                    this._showToast(`Error creating datatype: ${error.message}`, 'danger');
                });

        } catch (error) {
            console.error('Unexpected error in _createDatatype:', error);
        }
    }






    render() {
        return html`

<h1>Welcome ${this._currentUser?.name ?? "Unknown"}!</h1>
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
                    <uui-button look="primary" label="Create datatype" @click=${(e: Event) => this._createDatatype(e)}></uui-button>
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
            (doc) => html`
                            <li>
                                <uui-box headline="${doc.alias}" headline-variant="h5">
                                    <p>${doc.name}</p>
                                    <ul>
                                        ${doc.propertyTypes.map(
                (property) => html`<li>${property.name}</li>`
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

    static styles = [
        css`
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

        `,
    ];
}

export default McpsManageDashboardElement;

declare global {
    interface HTMLElementTagNameMap {
        'mcps-manage-dashboard-element': McpsManageDashboardElement;
    }
}

