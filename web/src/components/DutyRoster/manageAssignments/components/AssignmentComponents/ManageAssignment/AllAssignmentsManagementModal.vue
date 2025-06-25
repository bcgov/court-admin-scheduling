<template>
    <div>
        <b-modal v-model="showModal.show" no-close-on-backdrop hide-footer centered header-class="bg-white text-light" size="xl">
            <template v-slot:modal-header>
                <b-button
                    v-if="hasPermissionToAddAssignments"
                    variant="success"
                    size="sm"
                    :disabled="loadingData"
                    class="text-white px-4"
                    @click="createAssignments()">
                    Assignments +
                </b-button>
                <b-card class="border-0 mb-0" no-body :style="{opacity: (loadingData? 0.4 : 1)}">
                    <b-row style="margin:0.75rem 0 -0.65rem auto; border:1px solid #EFEFEF;">
                        <div
                            class="mx-2 p-0"
                            v-for="color in Object.keys(dutyColorsCode)" 
                            :key="color">
                            <b-row class="m-0 p-0">
                                <div :style="{backgroundColor:dutyColorsCode[color], width:'0.7rem', height:'0.7rem', borderRadius:'25px', margin:'0.1rem .2rem 0 0'}"/>
                                <div style="color:#000; font-size:9px; text-transform: capitalize; margin:0 0 0 0; padding:0"> {{color}}</div>
                            </b-row>
                        </div>
                    </b-row>
                </b-card>
                <!-- <div style="" class="h4 mt-1 mb-n2 p-0 text-dark text-center">{{shiftDate}}</div>  -->
                <b-button
                    size="sm"
                    variant="dark"
                    :disabled="loadingData"
                    class="text-white px-4"
                    @click="closeModal()">
                    Close
                </b-button>                            
            </template>
            <b-overlay :show="loadingData" rounded="sm" :opacity="0.9" spinner-large spinner-variant="primary">
                <b-card no-body style="border-bottom: 1px solid #EFEFEF ; font-size: 14px; margin-top:-1rem; user-select: none;" >	
                    <b-table
                        :items="dutyShiftAssignmentsWeek" 
                        :fields="fields"
                        sort-by="assignment"
                        :style="{ overflowX: 'scroll', marginBottom: '0px' }" 
                        small   
                        borderless                    
                        fixed>
                            <template v-slot:table-colgroup>
                                <col style="width:9rem">                         
                            </template>
                    
                            <template v-slot:cell(assignment) ="data"  >                            
                                <manage-duty-roster-assignment v-on:change="getData" :assignment="data.item" :weekview="true"/>
                            </template>
                                                    
                            <template v-slot:head()="data" >                            
                                <div v-if="data.column.substring(1,2)>=0" style="border:1px solid #EFEFEF;" class="m-0 bg-light text-center" >
                                    {{getBeautifyTime(Number(data.column.substring(1,2)))}}
                                </div>
                            </template>
                    
                            <template v-slot:cell()="data" >                                                                                      
                                <div v-if="data.item[data.field.key.substring(1,2)]" 
                                    class="text-center m-1 py-2 text-white rounded" 
                                    :style="{background:data.item.type.colorCode}"
                                    >
                                    <b-button v-if="hasPermissionToEditDuty" 
                                        style="width:1.1rem; height: 1.1rem; float:right; transform: translate(0px,2px);" 
                                        class="mx-1 my-0 py-0" 
                                        size="sm"
                                        variant="primary" 
                                        @click="editAssignmentDuty(data.item[data.field.key.substring(1,2)])"
                                        v-b-tooltip.hover                                
                                        title="Edit"
                                        ><b-icon
                                            icon="pencil-square" 
                                            font-scale="1.25" 
                                            variant="light" 
                                            style="transform: translate(-9px,-3px);"/>
                                    </b-button>
                                    {{ getAssignmentTime(data.item[data.field.key.substring(1,2)])}}
                                </div>                        
                            </template>

                    </b-table>               
                </b-card>
            </b-overlay>
        </b-modal>

        <create-assignments-modal :showModal="showCreateAssignments" v-on:change="getData" />
        <EditAssignmentDutyModal  :showModal="showEditAssignmentDuty" :duty="editingDuty" v-on:change="getData" />
    </div>    
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from "vuex-class";    
    import * as _ from 'underscore';
    import moment from 'moment-timezone';

    import "@store/modules/AssignmentScheduleInformation";
	const assignmentState = namespace("AssignmentScheduleInformation");

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    // import { userInfoType } from '@/types/common';
    // import { allEditingDutySlotsInfoType, manageAssignmentDutyInfoType, manageAssignmentsScheduleInfoType } from '@/types/DutyRoster';

    import CreateAssignmentsModal from './CreateAssignmentsModal.vue';
    import EditAssignmentDutyModal from './EditAssignmentDutyModal.vue'
    import ManageDutyRosterAssignment from './ManageDutyRosterAssignment.vue';
    import { manageAssignmentDutyInfoType, assignmentCardWeekInfoType, dutyRangeInfoType, assignDutyInfoType } from '@/types/DutyRoster';
    import { locationInfoType, userInfoType } from '@/types/common';


    @Component({
        components: {
            CreateAssignmentsModal,
            ManageDutyRosterAssignment,
            EditAssignmentDutyModal
        }
    })
    export default class AllAssignmentsManagementModal extends Vue {

        @Prop({required: true})
        showModal!: {show: boolean};

        @Prop({required: true})
        shiftDate!: string;

        @commonState.State
        public userDetails!: userInfoType;

        @assignmentState.State
        public dutyShiftAssignmentsWeek!: assignmentCardWeekInfoType[];

        @assignmentState.State
		public assignmentRangeInfo!: dutyRangeInfoType;

        @assignmentState.Action
        public UpdateManageDutiesModalID!: (newManageDutiesModalID: string) => void

        showCreateAssignments={show: false}
        showEditAssignmentDuty={show: false}
        hasPermissionToAddAssignments = false;
        hasPermissionToEditDuty = false;

        loadingData=false

        editingDuty = {} as assignDutyInfoType;

        fields =[
            {key:'assignment', label:'', thClass:'', tdClass:'p-0 m-0', thStyle:''},
            {key:'h0', label:'', thClass:'m-0 p-0', tdClass:'p-0 m-0 border-right', thStyle:''},
            {key:'h1', label:'', thClass:'m-0 p-0', tdClass:'p-0 m-0 border-right', thStyle:''},
            {key:'h2', label:'', thClass:'m-0 p-0', tdClass:'p-0 m-0 border-right', thStyle:''},
            {key:'h3', label:'', thClass:'m-0 p-0', tdClass:'p-0 m-0 border-right', thStyle:''},
            {key:'h4', label:'', thClass:'m-0 p-0', tdClass:'p-0 m-0 border-right', thStyle:''},
            {key:'h5', label:'', thClass:'m-0 p-0', tdClass:'p-0 m-0 border-right', thStyle:''},
            {key:'h6', label:'', thClass:'m-0 p-0', tdClass:'p-0 m-0 border-right', thStyle:''},
        ]

        dutyColorsCode={}

        mounted(){
            this.loadingData=false;
            this.dutyColorsCode=Vue.filter('WSColors')() 
            delete this.dutyColorsCode['Overtime']
            this.hasPermissionToAddAssignments = this.userDetails.permissions.includes("CreateAssignments");
            this.hasPermissionToEditDuty = this.userDetails.permissions.includes("EditDuties");
        }

        public createAssignments(){
            this.showCreateAssignments.show = true
        }

        public editAssignmentDuty(data){
            this.editingDuty=data
            // console.log(data)
            this.showEditAssignmentDuty.show=true;
        }

        public getBeautifyTime(day: number){
            return moment(this.assignmentRangeInfo.startDate).add(day, 'days').format('ddd DD MMM YYYY');
        }

        public getData(){
            this.loadingData=true
            this.$emit('change')
        }

        public getAssignmentTime(card){
            if(card?.assignment?.start)
                return card.assignment.start.substring(0,5) + ' - ' + card.assignment.end.substring(0,5)
            else
                return ''
        }

        public closeModal(){
            this.UpdateManageDutiesModalID("")
            this.showModal.show=false
        }
        
    }
</script>
<style scoped>   

    .card {
        border: white;
    }
</style>

