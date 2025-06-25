<template>
  <b-card bg-variant="white" class="home">
    <b-row class="bg-white">
      <b-col cols="8">
        <page-header pageHeaderText="Define Types"></page-header>
      </b-col>
      <b-col cols="2">
        <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 15rem">
          <b-form-select
            size="lg"
            v-model="selectedCategory"
            disabled
          >
            <b-form-select-option :value="categoryOption.value">
              {{ categoryOption.label }}
            </b-form-select-option>
          </b-form-select>
        </b-form-group>
      </b-col>
      <b-col cols="2">
        <div :class="expiredViewChecked?'bg-warning':''" :style="(expiredViewChecked?'width: 13.25rem;':'width: 15.25rem;')+'margin: .75rem 1rem 0 .5rem;'">
          <b-form-checkbox class="ml-2" v-model="expiredViewChecked" @change="getTypes()" size="lg" switch>
            {{ viewStatus }}
          </b-form-checkbox>
        </div>
      </b-col>
    </b-row>

    <loading-spinner v-if="!isDataMounted" />

    <b-card v-else no-body style="width: 50rem; margin: 0 auto 8rem auto">
      <b-card id="TypeError" no-body>
        <h2 v-if="typeError" class="mx-1 mt-2">
          <b-badge v-b-tooltip.hover :title="typeErrorMsgDesc" variant="danger">
            {{ typeErrorMsg }}
            <b-icon class="ml-3" icon="x-square-fill" @click="typeError = false" />
          </b-badge>
        </h2>
      </b-card>

      <div v-if="hasPermissionToCreateTypes">
        <b-card v-if="!addNewTypeForm">
          <b-button size="sm" variant="success" @click="addNewType">
            <b-icon icon="plus" /> Add
          </b-button>
        </b-card>
        <b-card v-else id="addTypeForm" class="my-3" :border-variant="addFormColor" style="border:2px solid" body-class="m-0 px-0 py-1">
          <add-types-form
            :formData="newType"
            :isCreate="true"
            @submit="saveType"
            @cancel="closeTypeForm"
          />
        </b-card>
      </div>

      <div>
        <b-card no-body border-variant="white" bg-variant="white" v-if="!typeList.length" style="width: 50rem; margin: 0 auto 8rem auto">
          <span class="text-muted ml-4 my-5">No Types exist.</span>
        </b-card>

        <b-card v-else no-body border-variant="light" bg-variant="white" style="width: 50rem; margin: 0 auto 8rem auto">
          <b-table
            :key="updateTable"
            :items="typeList"
            :fields="fields"
            sort-icon-left
            head-row-variant="primary"
            borderless
            small
            v-sortDefineType
            responsive="sm"
          >
            <template v-slot:table-colgroup>
              <col style="width:4rem">
              <col>
              <col>
              <col>
              <col style="width:4rem">
              <col style="width:6rem">
            </template>

            <template v-slot:head(sortOrder)>
              <span></span>
            </template>
            <template v-slot:head(name)>
              <span>Name</span>
            </template>
            <template v-slot:head(description)>
              <span>Label</span>
            </template>
            <template v-slot:head(abbreviation)>
              <span>Abbreviation</span>
            </template>
            <template v-slot:head(displayColor)>
              <span>Colour</span>
            </template>
            <template v-slot:head(edit)>
              <span></span>
            </template>

            <template v-slot:cell(sortOrder)="data">
              <span v-if="!data.item.isSystem && !data.item._rowVariant">
                <b-icon class="handle ml-3" icon="arrows-expand" />
              </span>
            </template>
            <template v-slot:cell(displayColor)="data">
              <span>
                <span :style="{background: data.item.displayColor, borderRadius: '50%', display: 'inline-block', width: '1.5em', height: '1.5em', border: '1px solid #ccc'}"></span>
              </span>
            </template>
            <template v-slot:cell(edit)="data">
              <b-button v-if="!data.item.isSystem && !data.item._rowVariant"
                class="ml-2 px-1"
                style="padding: 1px 2px 1px 2px;"
                size="sm"
                v-b-tooltip.hover
                title="Expire"
                variant="warning"
                :disabled="data.item.isSystem"
                @click="confirmExpireType(data.item)">
                <b-icon icon="clock" font-scale="1" variant="white" />
              </b-button>
              <b-button v-if="data.item._rowVariant"
                class="my-0 ml-2 py-0 px-1"
                size="sm"
                variant="warning"
                @click="confirmUnexpireType(data.item)">
                <b-icon icon="arrow-counterclockwise" font-scale="1.25" variant="danger" />
              </b-button>
              <b-button v-if="!data.item.isSystem"
                :disabled="!!data.item._rowVariant"
                class="my-0 py-0"
                size="sm"
                variant="transparent"
                @click="editType(data)">
                <b-icon icon="pencil-square" font-scale="1.25" variant="primary" />
              </b-button>
            </template>
            <template v-slot:row-details="data">
              <b-card :id="'#Type-'+data.item.id" body-class="m-0 px-0 py-1" :border-variant="addFormColor" style="border:2px solid">
                <add-types-form
                  :formData="data.item"
                  :isCreate="false"
                  :formDisabled="data.item.isSystem || !!data.item._rowVariant"
                  @submit="saveType"
                  @cancel="() => closeEditRow(data)"
                />
              </b-card>
            </template>
          </b-table>
        </b-card>
      </div>
    </b-card>

    <b-modal v-model="confirmExpire" id="bv-modal-confirm-expire" header-class="bg-warning text-light">
      <template v-slot:modal-title>
        <h2 class="mb-0 text-light">{{ deleteType == 'expire' ? 'Confirm Expire Type' : 'Confirm Unexpire Type' }}</h2>
      </template>
      <h4 v-if="deleteType == 'expire'">Are you sure you want to Expire the "{{ typeToDelete.name }}" type?</h4>
      <h4 v-else>Are you sure you want to Unexpire the "{{ typeToDelete.name }}" type?</h4>
      <template v-slot:modal-footer>
        <b-button variant="danger" @click="deleteTypeAction()">Confirm</b-button>
        <b-button variant="primary" @click="confirmExpire = false">Cancel</b-button>
      </template>
      <template v-slot:modal-header-close>
        <b-button variant="outline-warning" class="text-light closeButton" @click="confirmExpire = false">&times;</b-button>
      </template>
    </b-modal>

    <b-modal v-model="openErrorModal" header-class="bg-warning text-light">
      <b-card class="h4 mx-2 py-2">
        <span class="p-0">{{ errorText }}</span>
      </b-card>
      <template v-slot:modal-footer>
        <b-button variant="primary" @click="openErrorModal=false">Ok</b-button>
      </template>
      <template v-slot:modal-header-close>
        <b-button variant="outline-warning" class="text-light closeButton" @click="openErrorModal=false">&times;</b-button>
      </template>
    </b-modal>
  </b-card>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import PageHeader from "@components/common/PageHeader.vue";
import sortDefineType from './utils/sortDefineType';
import { defineTypeInfoType } from '@/types/ManageTypes';
import AddTypesForm from './AddTypesForm.vue';

@Component({
  components: {
    PageHeader,
    AddTypesForm
  },
  directives: {
    sortDefineType
  }
})
export default class DefineTypes extends Vue {
  selectedCategory = 0; // Only Assignment for now
  categoryOption = { value: 0, label: 'Assignment' };

  isDataMounted = false;
  addNewTypeForm = false;
  addFormColor = 'dark';
  updateTable = 0;

  expiredViewChecked = false;
  typeError = false;
  typeErrorMsg = '';
  typeErrorMsgDesc = '';
  errorText = '';
  openErrorModal = false;

  confirmExpire = false;
  deleteType = 'expire';
  typeToDelete = {} as defineTypeInfoType;

  hasPermissionToCreateTypes = true; // Set your permission logic as needed

  typeList: defineTypeInfoType[] = [];
  newType: defineTypeInfoType = { id: 0, name: '', description: '', abbreviation: '', displayColor: '#189fd4' };

  fields = [
    { key: 'sortOrder', label: '', sortable: false, tdClass: 'border-top' },
    { key: 'name', label: 'Name', sortable: false, tdClass: 'border-top' },
    { key: 'description', label: 'Label', sortable: false, tdClass: 'border-top' },
    { key: 'abbreviation', label: 'Abbreviation', sortable: false, tdClass: 'border-top' },
    { key: 'displayColor', label: 'Colour', sortable: false, tdClass: 'border-top' },
    { key: 'edit', label: '', sortable: false, tdClass: 'border-top' }
  ];

  // --- Sorting ---
  saveOrderFlag = false;

  mounted() {
    this.getTypes();
  }

  get viewStatus() {
    return this.expiredViewChecked ? 'All Types' : 'Active Types';
  }

  getTypes() {
    this.isDataMounted = false;
    const url = this.expiredViewChecked
      ? '/api/lookuptype/all?category=' + this.selectedCategory
      : '/api/lookuptype/actives?category=' + this.selectedCategory;
    this.$http.get(url)
      .then(response => {
        this.typeList = this.extractTypes(response.data);
        this.isDataMounted = true;
      }, err => {
        this.errorText = err.response.statusText + ' ' + err.response.status;
        this.openErrorModal = true;
        this.isDataMounted = true;
      });
  }

  extractTypes(typesJson: any[]): defineTypeInfoType[] {
    const list: defineTypeInfoType[] = [];
    for (const typeJson of typesJson) {
      const type: defineTypeInfoType = {
        id: typeJson.id,
        name: typeJson.name,
        description: typeJson.description,
        abbreviation: typeJson.abbreviation,
        displayColor: typeJson.displayColor,
        isSystem: typeJson.isSystem,
        expiryDate: typeJson.expiryDate,
        sortOrder: typeJson.sortOrder,
        _rowVariant: typeJson.expiryDate ? 'info' : ''
      };
      list.push(type);
    }
    return list;
  }

  // Call this after drag-and-drop or sort change
  saveSortOrders() {
  // Only send non-system items for sorting, system items always stay at the top
  const sortOrders = this.typeList
    .filter(x => !x.isSystem && !x._rowVariant)
    .map((item, idx) => ({
      Id: item.id,
      SortOrder: idx + this.typeList.filter(x => x.isSystem).length // offset by system items
    }));
  (this.$http.put('/api/lookuptype/sort', sortOrders) as Promise<any>)
    .then(() => { /* Optionally refresh list or show success */ })
    .catch(err => {
      this.typeErrorMsg = err.response.data.error;
      this.typeErrorMsgDesc = err.response.data.error;
      this.typeError = true;
    });
  }

  // --- Add/Edit handlers ---
  addNewType() {
    this.addNewTypeForm = true;
    this.newType = { id: 0, name: '', description: '', abbreviation: '', displayColor: '#189fd4' };
    this.$nextTick(() => { location.href = '#addTypeForm'; });
  }

  closeTypeForm() {
    this.addNewTypeForm = false;
    this.addFormColor = 'secondary';
  }
  closeEditRow(data: any) {
  if (data && typeof data.toggleDetails === 'function') {
    data.toggleDetails();
  }
}

  saveType(formData: defineTypeInfoType, isCreate: boolean) {
    const body = {
      name: formData.name,
      description: formData.description,
      abbreviation: formData.abbreviation,
      displayColor: formData.displayColor,
      category: this.selectedCategory
    };
    const req = isCreate
      ? this.$http.post('/api/lookuptype', body)
      : this.$http.put('/api/lookuptype', { ...body, id: formData.id });
    req.then(() => {
      this.getTypes();
      this.closeTypeForm();
    }, err => {
      this.typeErrorMsg = err.response.data.error;
      this.typeErrorMsgDesc = err.response.data.error;
      this.typeError = true;
    });
  }

  // For edit in row-details
  editType(data: any) {
    if (this.addNewTypeForm) {
      location.href = '#addTypeForm';
      this.addFormColor = 'danger';
    } else if (data.detailsShowing) {
      location.href = '#Type-' + data.item.id;
      this.addFormColor = 'danger';
    } else {
      data.toggleDetails();
    }
  }

  saveTypeEdit(item: defineTypeInfoType) {
    const body = {
      id: item.id,
      name: item.name,
      description: item.description,
      abbreviation: item.abbreviation,
      displayColor: item.displayColor
    };
    this.$http.put('/api/lookuptype', body)
      .then(response => {
        this.getTypes();
        this.closeTypeForm();
      }, err => {
        this.typeErrorMsg = err.response.data.error;
        this.typeErrorMsgDesc = err.response.data.error;
        this.typeError = true;
      });
  }

  confirmExpireType(type: defineTypeInfoType) {
    this.typeToDelete = type;
    this.deleteType = 'expire';
    this.confirmExpire = true;
  }

  confirmUnexpireType(type: defineTypeInfoType) {
    this.typeToDelete = type;
    this.deleteType = 'unexpire';
    this.confirmExpire = true;
  }

  deleteTypeAction() {
    this.confirmExpire = false;
    const url = `/api/lookuptype/${this.typeToDelete.id}/${this.deleteType}`;
    this.$http.put(url)
      .then(response => {
        this.getTypes();
      }, err => {
        this.typeErrorMsg = err.response.data.error;
        this.typeErrorMsgDesc = err.response.data.error;
        this.typeError = true;
      });
  }
}
</script>

<style scoped>
.card {
  border: white;
}
</style>