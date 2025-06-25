<template>
  <div>
    <b-table-simple small borderless>
      <b-tbody>
        <b-tr>
          <b-td>
            <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 10rem">
              <label class="h6 ml-1 mb-0 pb-0"> Name: </label>
              <b-form-input
                size="sm"
                v-model="formData.name"
                type="text"
                placeholder="Enter name"
                :disabled="formDisabled || !isCreate"
                required
              />
            </b-form-group>
          </b-td>
          <b-td>
            <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 10rem">
              <label class="h6 ml-1 mb-0 pb-0"> Description: </label>
              <b-form-input
                size="sm"
                v-model="formData.description"
                type="text"
                placeholder="Enter description"
                :disabled="formDisabled"
                required
              />
            </b-form-group>
          </b-td>
          <b-td>
            <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 8rem">
              <label class="h6 ml-1 mb-0 pb-0"> Abbreviation: </label>
              <b-form-input
                size="sm"
                v-model="formData.abbreviation"
                type="text"
                placeholder="Enter abbreviation"
                :disabled="formDisabled"
                required
              />
            </b-form-group>
          </b-td>
          <b-td>
            <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 8rem">
              <label class="h6 ml-1 mb-0 pb-0"> Display Colour: </label>
              <div class="d-flex align-items-center">
                <input
                  type="color"
                  v-model="formData.displayColor"
                  :disabled="formDisabled"
                  class="color-circle ml-2"
                  :style="{ background: formData.displayColor }"
                />
              </div>
            </b-form-group>
          </b-td>
          <b-td>
            <b-button
              style="margin: 1.5rem .5rem 0 0 ; padding:0 .5rem 0 .5rem;"
              variant="secondary"
              @click="closeForm"
              :disabled="formDisabled"
            >
              Cancel
            </b-button>
            <b-button
              style="margin: 1.5rem 0 0 0; padding:0 0.7rem 0 0.7rem;"
              variant="success"
              @click="saveForm"
              :disabled="formDisabled"
            >
              Save
            </b-button>
          </b-td>
        </b-tr>
      </b-tbody>
    </b-table-simple>

    <b-modal v-model="showCancelWarning" id="bv-modal-type-cancel-warning" header-class="bg-warning text-light">
      <template v-slot:modal-title>
        <h2 class="mb-0 text-light">Unsaved Changes</h2>
      </template>
      <p>Are you sure you want to cancel without saving your changes?</p>
      <template v-slot:modal-footer>
        <b-button variant="secondary" @click="showCancelWarning = false">No</b-button>
        <b-button variant="success" @click="confirmedCloseForm">Yes</b-button>
      </template>
      <template v-slot:modal-header-close>
        <b-button variant="outline-warning" class="text-light closeButton" @click="showCancelWarning = false">&times;</b-button>
      </template>
    </b-modal>
  </div>
</template>

<script lang="ts">
import { Component, Vue, Prop, Watch } from 'vue-property-decorator';
import { defineTypeInfoType } from '@/types/ManageTypes';

@Component
export default class AddTypesForm extends Vue {
  @Prop({ required: true }) formData!: defineTypeInfoType;
  @Prop({ required: true }) isCreate!: boolean;
  @Prop({ default: false }) formDisabled!: boolean;

  showCancelWarning = false;
  originalData: defineTypeInfoType = { ...this.formData };

  @Watch('formData', { immediate: true, deep: true })
  onFormDataChanged() {
    this.originalData = { ...this.formData };
  }

  saveForm() {
    this.$emit('submit', { ...this.formData }, this.isCreate);
  }

  closeForm() {
    if (this.isChanged()) {
      this.showCancelWarning = true;
    } else {
      this.confirmedCloseForm();
    }
  }

  confirmedCloseForm() {
    this.showCancelWarning = false;
    this.$emit('cancel');
  }

  isChanged() {
    return (
      this.formData.name !== this.originalData.name ||
      this.formData.description !== this.originalData.description ||
      this.formData.displayColor !== this.originalData.displayColor ||
      this.formData.abbreviation !== this.originalData.abbreviation
    );
  }
}
</script>

<style scoped>
.color-circle {
  display: inline-block;
  width: 2.2rem;
  height: 2.2rem;
  border-radius: 50%;
  border: 1px solid #ccc;
  box-shadow: 0 0 2px #aaa;
}
td {
  margin: 0rem 0.5rem 0.1rem 0rem;
  padding: 0rem 0.5rem 0.1rem 0rem;
  background-color: white;
}
</style>