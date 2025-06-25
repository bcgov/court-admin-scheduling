<template>
    <div>
        <b-modal v-model="showModal.show" no-close-on-backdrop centered header-class="bg-primary text-light">
            <template v-slot:modal-title>
                <h2 class="mb-0 text-light"> Creating Assignment </h2>
            </template>

            <b-card no-body style="font-size: 14px; user-select: none;" >

                <b-card id="AssignmentError" no-body>
                    <h2 v-if="assignmentError" class="mx-1 mt-2"
                        ><b-badge v-b-tooltip.hover
                            :title="assignmentErrorMsgDesc"
                            variant="danger"> {{assignmentErrorMsg}}
                            <b-icon class="ml-3"
                                icon = x-square-fill
                                @click="assignmentError = false"
                    /></b-badge></h2>
                </b-card>

                <b-row class="mx-1 mt-0 mb-2 p-0">
                    <b-form-group class="my-auto ml-auto" style="width: 8.6rem">	
                        <b-form-checkbox
                            size="sm"									
                            v-model="nonReoccuring"
                            @change="toggleReoccurring">
                                Non Reoccuring
                        </b-form-checkbox>						
                    </b-form-group>
                </b-row>              

                <b-row class="mx-1 my-0 p-0">
                    <b-form-group class="mr-1" style="width: 12rem">
                        <label class="h6 m-0 p-0">Assignment Category<span class="text-danger">*</span></label>
                        <b-form-select 
                            size="sm"
                            @change="loadSubTypes"
                            v-model="assignment.type"
                            :state = "selectedTypeState?null:false">
                                <b-form-select-option
                                    v-for="type in assignmentTypeOptions"
                                    :key="type.name"
                                    :value="type">
                                            {{type.label}}
                                </b-form-select-option>
                        </b-form-select>
                    </b-form-group>
                    <b-form-group class="ml-4" style="width: 14.35rem">
                        <label class="h6 my-0 ml-1 p-0">Assignment Sub category<span class="text-danger">*</span></label>
                        <b-form-select 
                            size="sm"
                            :disabled="!isSubTypeDataReady"
                            v-model="assignment.lookupCodeId"
                            :state = "selectedSubTypeState?null:false">
                                <b-form-select-option
                                    v-for="subType in assignmentSubTypeOptions"
                                    :key="subType.id"
                                    :value="subType.id">
                                            {{subType.code}}
                                </b-form-select-option>
                        </b-form-select>
                    </b-form-group>
                </b-row>

                <b-row class="mx-1 my-0 p-0">
                    <b-form-group class="mr-1" style="width: 12rem">
                        <label class="h6 m-0 p-0">Name</label>
                        <b-form-input 
                        size="sm"
                            v-model="assignment.name" 
                            placeholder="Enter Name" 
                            :state = "nameState?null:false">
                        </b-form-input>
                    </b-form-group>
                </b-row>

                <b-row v-if="nonReoccuring" class="mx-1 my-1 p-0">
                    <b-form-group class="mr-1" style="width: 12rem">					
                        <label class="h6 m-0 p-0"> From <span class="text-danger">*</span></label>
                        <b-form-datepicker
                            tabindex="2"
                            class="mb-1"
                            size="sm"
                            v-model="selectedStartDate"
                            placeholder="Start Date*"
                            :state = "startDateState?null:false"
                            :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                            @context="startDatePicked"
                            locale="en-US">
                        </b-form-datepicker>
                    </b-form-group>
                    <b-form-group class="ml-4" style="width: 14.35rem">
                        <label class="h6 m-0 p-0"> To<span class="text-danger">*</span></label>
                        <b-form-datepicker
                            tabindex="3"
                            class="mb-1 mt-0 pt-0"
                            size="sm"
                            v-model="selectedEndDate"
                            placeholder="End Date*"
                            :state = "endDateState?null:false"                                    
                            :date-format-options="{ year: 'numeric', month: 'short', day: '2-digit' }"
                            @context="endDatePicked"
                            locale="en-US">
                        </b-form-datepicker>
                    </b-form-group>
                </b-row>

                <b-row class="mx-1 mt-3 mb-0">
                    <b-form-group class="bg-light">							
                        <b-row>
                            <label class="h6 ml-3 mr-5">Days<span class="text-danger">*</span></label>								
                            <b-form-checkbox
                                size="sm"
                                class="ml-auto mr-4 text-jail"									
                                v-model="weekDaysSelected"								
                                :disabled="selectWeekDisabled"
                                @change="toggleWeekDays">
                                    Select Week Days
                            </b-form-checkbox>    
                            <b-form-checkbox
                                size="sm"
                                class="ml-auto mr-4 text-court"									
                                v-model="allDaysSelected"								
                                :disabled="selectAllDisabled"
                                @change="toggleAllDays">
                                    Select All
                            </b-form-checkbox>
                        </b-row>
                        <b-form-checkbox-group
                            size="sm"			
                            v-model="selectedDays"								
                            :state = "selectedDayState?null:false">
                                <b-form-checkbox
                                    @change="weekdaysChanged"									
                                    :class="day.diff? 'ml-2 pl-3' :'ml-1 pl-4' +'align-middle'"
                                    v-for="day in dayOptions"
                                    :disabled="!day.enabled"
                                    :key="day.diff"
                                    :value="day.diff">
                                        {{day.name}}
                                </b-form-checkbox>
                        </b-form-checkbox-group>
                    </b-form-group>
                </b-row>

                <b-row class="mx-auto my-0 p-0">
                    <b-form-group class="mr-3" style="width: 7rem">
                        <label class="h6 m-0 p-0">From<span class="text-danger">*</span></label>
                        <b-form-input
                            v-model="selectedStartTime"
                            @click="startTimeState=true"
                            size="sm"
                            type="text"
                            autocomplete="off"
                            @paste.prevent
                            :formatter="timeFormat"
                            placeholder="HH:MM"
                            :state = "startTimeState?null:false"
                        ></b-form-input>
                    </b-form-group>

                    <b-form-group style="width: 7rem;">
                        <label class="h6 m-0 p-0">To<span class="text-danger">*</span></label>
                        <b-form-input
                            v-model="selectedEndTime"
                            @click="endTimeState=true"
                            size="sm"
                            type="text"
                            autocomplete="off"
                            @paste.prevent
                            :formatter="timeFormat"
                            placeholder="HH:MM"
                            :state = "endTimeState?null:false"
                        ></b-form-input>
                    </b-form-group>						
                </b-row>
                <b-row class="mx-auto my-0 p-0">
                    <b-form-group class="m-0" style="width: 28.5rem">
                        <label class="h6 m-0 p-0">Comment</label>
                        <b-form-input
                            v-model="selectedComment"
                            size="sm"
                            type="text"
                            :formatter="commentFormat"                            
                        ></b-form-input>
                    </b-form-group>                                    
                </b-row>
            </b-card>

            <template v-slot:modal-footer>
                <b-button
                        size="sm"
                        variant="secondary"
                        @click="closeAssignmentWindow()"
                ><b-icon-x style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-x>Cancel</b-button>
                <b-button
                        size="sm"
                        variant="success"
                        @click="saveAssignment()"
                ><b-icon-check2 style="padding:0; vertical-align: middle; margin-right: 0.25rem;"></b-icon-check2>Save</b-button>
            </template>
            <template v-slot:modal-header-close>
                <b-button
                    variant="outline-primary"
                    class="text-light closeButton"
                    @click="closeAssignmentWindow()"
                    >
                    &times;</b-button>
            </template>
        </b-modal>

        <b-modal v-model="showCancelWarning" id="bv-modal-assignment-cancel-warning" header-class="bg-warning text-light">            
            <template v-slot:modal-title>
                <h2 class="mb-0 text-light"> Unsaved New Assignments </h2>                                 
            </template>
            <p>Are you sure you want to cancel without saving your changes?</p>
            <template v-slot:modal-footer>
                <b-button variant="secondary" @click="$bvModal.hide('bv-modal-assignment-cancel-warning')"                   
                >No</b-button>
                <b-button variant="success" @click="confirmedCloseAssignmentWindow()"
                >Yes</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-assignment-cancel-warning')"
                >&times;</b-button>
            </template>
        </b-modal>

        <b-modal v-model="openErrorModal" header-class="bg-warning text-light">
            <b-card class="h4 mx-2 py-2">
                <span class="p-0">{{errorText}}</span>
            </b-card>            
            <template v-slot:modal-footer>
                <b-button variant="primary" @click="openErrorModal=false">Ok</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="openErrorModal=false"
                >&times;</b-button>
            </template>
        </b-modal>
    </div>    
</template>

<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import { namespace } from "vuex-class";    
    import * as _ from 'underscore';
    import moment from 'moment-timezone';

    // import "@store/modules/AssignmentScheduleInformation";
    // const assignmentState = namespace("AssignmentScheduleInformation");

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    // import { userInfoType } from '@/types/common';
    // import { allEditingDutySlotsInfoType, manageAssignmentDutyInfoType, manageAssignmentsScheduleInfoType } from '@/types/DutyRoster';

    import AddAssignmentSlotForm from './AddAssignmentSlotForm.vue';
    import { manageAssignmentDutyInfoType, assignmentInfoType, assignmentSubTypeInfoType } from '@/types/DutyRoster';
    import { locationInfoType, userInfoType } from '@/types/common';

    @Component({
        components: {
            // AddAssignmentSlotForm
        }
    })
    export default class CreateAssignmentsModal extends Vue {

        @Prop({required: true})
        showModal!: {show: boolean};


        @commonState.State
        public location!: locationInfoType;

        // @commonState.State
        // public userDetails!: userInfoType;


        selectedDate = '';
        selectedDateBegin = '';
        selectedDateEnd = '';
        datePickerOpened = false;
        userIsAdmin = false;

        selectedStartTime = '';
        selectedEndTime = '';
        selectedStartDate = '';
        selectedEndDate = '';
        selectedDays: number[] = [];
        showAssignmentDetails = false;
        showCancelWarning = false;
        isSubTypeDataReady = false;
        nonReoccuring = false;

        selectedComment = '';

        assignment = {} as assignmentInfoType;

        nameState = true;
        selectedTypeState = true;
        selectedSubTypeState = true;
        startDateState = true;
        endDateState = true;	
        startTimeState = true;
        endTimeState = true;
        selectedDayState = true;		
        allDaysSelected = false;
        weekDaysSelected = false;
        
        assignmentError = false;
        assignmentErrorMsg = '';
        assignmentErrorMsgDesc = '';
       
        dayOptions = [
            {name:'Sun', diff:0, enabled: true},
            {name:'Mon', diff:1, enabled: true},
            {name:'Tue', diff:2, enabled: true},
            {name:'Wed', diff:3, enabled: true},
            {name:'Thu', diff:4, enabled: true},
            {name:'Fri', diff:5, enabled: true},
            {name:'Sat', diff:6, enabled: true},
        ]

        assignmentTypeOptions: { name: string; code: number; label: string }[] = [];

        assignmentSubTypeOptions = [] as assignmentSubTypeInfoType[];

        errorText=''
        openErrorModal=false;

        mounted(){
            this.fetchAssignmentTypeTabs()
        }
        public async fetchAssignmentTypeTabs() {
            const url = '/api/lookuptype/actives?category=Assignment';
            try {
                const response = await this.$http.get(url);
                    if (response.data && Array.isArray(response.data)) {
                        this.assignmentTypeOptions = response.data.map((item: any) => ({
                            name: item.name,
                            code: item.code,
                            label: item.description
                        }));
                    }
            } catch (err) {
                this.errorText = `Error fetching assignment types`;
                this.openErrorModal = true;
            }
        }
        // mounted(){
        //     this.shiftDay = moment(this.shiftDate).tz(this.location.timezone).day();
        //     this.hasPermissionToEditDuty = this.userDetails.permissions.includes("EditDuties");            
        //     this.hasPermissionToAddAssignDuty = this.userDetails.permissions.includes("CreateAndAssignDuties");
        // }

        // public manageAssignments(){
        //     this.showManageAssignments.show = true
        // }

        public closeAssignmentWindow(){
            if(this.isChanged())
                this.showCancelWarning = true;
            else
                this.confirmedCloseAssignmentWindow();
        }

        public confirmedCloseAssignmentWindow(){
            this.resetAssignmentWindowState();
            this.showCancelWarning = false;
            this.showModal.show = false;
        }

        public saveAssignment() {
            let requiredError = false;
            // if (!this.assignment.name) {
            // 	this.nameState = false;
            // 	requiredError = true;
            // } else {
            // 	this.nameState = true;
            // }
            if (!this.assignment.type) {
                this.selectedTypeState = false;
                requiredError = true;
            } else {
                this.selectedTypeState = true;
            }
            if (!this.assignment.lookupCodeId) {
                this.selectedSubTypeState = false;
                requiredError = true;
            } else {
                this.selectedSubTypeState = true;
            }
            if (this.nonReoccuring && !this.selectedStartDate) {
                this.startDateState = false;
                requiredError = true;
            } else {
                this.startDateState = true;
            }
            if (this.nonReoccuring && !this.selectedEndDate) {
                this.endDateState = false;
                requiredError = true;
            } else {				
                this.endDateState = true;
            }
            if (!this.selectedStartTime) {
                this.startTimeState = false;
                requiredError = true;
            } else {
                this.selectedStartTime = Vue.filter('autoCompleteTime')(this.selectedStartTime);					
                this.startTimeState = true;
            }
            if (!this.selectedEndTime) {
                this.endTimeState = false;
                requiredError = true;
            } else {
                this.selectedEndTime = Vue.filter('autoCompleteTime')(this.selectedEndTime);					
                this.endTimeState = true;
            }
            if (this.selectedStartTime && this.selectedEndTime && this.selectedStartTime >= this.selectedEndTime ) {
                this.startTimeState = false;
                this.endTimeState = false;
                requiredError = true;
            } 
            if (this.selectedDays.length < 1) {
                this.selectedDayState = false;
                requiredError = true;
            } else {
                this.selectedDayState = true;
            }
            if (!requiredError) {
                    
                    this.createAssignment();
            } else {
                    console.log('Error required')
            }
        }


        public isChanged(){
            if( this.assignment.name ||
                this.assignment.type ||
                this.selectedComment ||
                this.selectedStartTime || this.selectedEndTime ||
                this.nonReoccuring || this.selectedDays.length >0) return true;
            return false;           
        }

        public resetAssignmentWindowState() {
            this.assignment = {} as assignmentInfoType;
            this.ClearFormState();
        }

        public ClearFormState(){
            this.allDaysSelected = false;
            this.weekDaysSelected = false;
            this.assignment = {} as assignmentInfoType;
            this.selectedStartTime ='';
            this.selectedEndTime = '';
            this.selectedStartDate = '';
            this.selectedEndDate = '';
            this.selectedDays = [] ;
            this.nameState = true;	
            this.selectedTypeState = true;
            this.selectedSubTypeState = true;
            this.selectedDayState = true;
            this.startTimeState = true;
            this.endTimeState = true;
            this.startDateState = true;
            this.endDateState = true;
            this.assignmentError = false;
            this.assignmentErrorMsg = '';
            this.assignmentErrorMsgDesc = '';
            this.nonReoccuring = false;
            this.enableAllDayOptions();
            this.selectedComment = '';
        }

        public enableAllDayOptions() {
            for (let dayOfWeek = 0; dayOfWeek < this.dayOptions.length; dayOfWeek++) {
                this.dayOptions[dayOfWeek].enabled = true;
            }
        }

        public createAssignment() {
            this.assignment.sunday = this.selectedDays.includes(0)
            this.assignment.monday = this.selectedDays.includes(1)
            this.assignment.tuesday = this.selectedDays.includes(2)
            this.assignment.wednesday = this.selectedDays.includes(3)
            this.assignment.thursday = this.selectedDays.includes(4)
            this.assignment.friday = this.selectedDays.includes(5)
            this.assignment.saturday = this.selectedDays.includes(6);

            this.assignment.locationId = this.location.id;
            this.assignment.timezone = this.location.timezone;			

            if (this.nonReoccuring) {
                this.assignment.adhocStartDate = moment.tz(this.selectedStartDate, this.location.timezone).utc().format();
                this.assignment.adhocEndDate = moment.tz(this.selectedEndDate, this.location.timezone).utc().format();
            } else {
                this.assignment.adhocStartDate = null;
                this.assignment.adhocEndDate = null;
            }

            this.assignment.start = this.selectedStartTime;
            this.assignment.end = this.selectedEndTime;

            this.assignment.comment = this.selectedComment;

            const body = this.assignment;	
            const url = 'api/assignment';
            this.$http.post(url, body )
                .then(response => {
                    if(response.data){
                        this.$emit('change'); 
                        this.confirmedCloseAssignmentWindow();							                          
                    }
                }, err => {
                    const errMsg = err.response.data;
                    this.assignmentErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.assignmentErrorMsgDesc = errMsg;
                    this.assignmentError = true;
                })
        }
        
        public timeFormat(value , event){
            return Vue.filter('timeFormat')(value , event)
        }

        public commentFormat(value) {
            return value.slice(0,100);
        }

        public toggleReoccurring(checked) {
            this.nonReoccuring = checked;
            if (checked) {
                this.selectedDays = [];				
            } else {
                this.enableAllDayOptions();
            }
        }

        public loadSubTypes(type) {
            Vue.nextTick(()=>{
                this.isSubTypeDataReady = false;
                const url = 'api/managetypes?codeType='+ type.name +'&locationId='+this.location.id+'&showExpired=false';
                this.$http.get(url)
                    .then(response => {
                        if(response.data){
                            this.extractSubTypes(response.data);
                        }
                    },err => {
                        this.errorText = err.response.statusText+' '+err.response.status + '  - ' + moment().format();
                        if (err.response.status != '401') {
                            this.openErrorModal=true;
                        }  
                    })
            });
        }

        public extractSubTypes(subTypesJson) {
            this.assignmentSubTypeOptions = [];
            for(const subTypeJson of subTypesJson) {
                const subType = {} as assignmentSubTypeInfoType;
                subType.id = subTypeJson.id;
                subType.code = subTypeJson.code;
                this.assignmentSubTypeOptions.push(subType)
            }
            this.isSubTypeDataReady = true;
        }



        public startDatePicked(){
            this.startDateState = true;
            this.endDateState = true;
            this.toggleAllDays(false);
            this.toggleWeekDays(false);
            this.selectedDays = [] ;
            this.selectedEndDate = this.selectedStartDate;
            this.disableOutOfRangeDays();			
        }
        
        public endDatePicked(){
            this.toggleAllDays(false);
            this.toggleWeekDays(false);
            this.selectedDays = [] ;            
            if (this.selectedStartDate.length) {
                this.disableOutOfRangeDays();
            }
        }

        public toggleAllDays(checked) {
            this.weekDaysSelected = checked ? true: false;
            this.selectedDays = checked ? [0,1,2,3,4,5,6] : [];
        }

        public toggleWeekDays(checked) {
            this.allDaysSelected = false;
            this.selectedDays = checked ? [1,2,3,4,5] : [];
        }

        public disableOutOfRangeDays() {
            const numberOfDaysInRange = moment(this.selectedEndDate).diff(moment(this.selectedStartDate), 'days');
            if (numberOfDaysInRange < 7) {
                const daysInRange: number[] = [];					

                for (let i = 0; i <= numberOfDaysInRange; i++) {
                    const dayOfWeek = moment(this.selectedStartDate).add(i, 'days').day();
                    daysInRange.push(dayOfWeek)
                }

                for (let dayOfWeek = 0; dayOfWeek < this.dayOptions.length; dayOfWeek++) {
                    if (!daysInRange.includes(dayOfWeek)) {
                        this.dayOptions[dayOfWeek].enabled = false;
                    } else {
                        this.dayOptions[dayOfWeek].enabled = true;
                    }
                }
            } else {
                this.enableAllDayOptions();
            }
        }

        public weekdaysChanged(){
            Vue.nextTick(()=>{
                this.allDaysSelected = this.selectedDays.length==7? true: false
                this.weekDaysSelected = this.selectedDays.includes(1) && this.selectedDays.includes(2) 
                                        && this.selectedDays.includes(3) && this.selectedDays.includes(4) 
                                        && this.selectedDays.includes(5);
            })
        }

        get selectAllDisabled(){
            for(const day of this.dayOptions)
                if(!day.enabled) return true;
            return false;
        }

        get selectWeekDisabled(){
            for(const day in this.dayOptions)
                if(!this.dayOptions[day].enabled && this.dayOptions[day].diff !=0 && this.dayOptions[day].diff !=6) return true;
            return false;
        }



    }
</script>
<style scoped lang="scss">

    .card {
        border: white;
    }
</style>
