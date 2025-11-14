<template>
    <b-card bg-variant="white" class="home">    
        <b-row class="bg-white">
            <b-col cols="8">
                <page-header :pageHeaderText="location.name+' '+sectionHeader"></page-header>                
            </b-col>
            <b-col cols="2">
                <b-form-group style="margin: 0.25rem 0 0 0.5rem;width: 15rem"> 
                    <b-form-select
                        size = "lg"
                        @change="changeAssignment"
                        v-model="selectedAssignmentType">                            
                            <b-form-select-option
                                v-for="assignmentType in assignmentTypeTabs" 
                                :key="assignmentType.name"                                
                                :value="assignmentType">
                                    {{assignmentType.label}}
                            </b-form-select-option>     
                    </b-form-select>
                </b-form-group>
            </b-col>
            <b-col cols="2" >
                <div :class="expiredViewChecked?'bg-warning':''" :style="(expiredViewChecked?'width: 13.25rem;':'width: 15.25rem;')+'margin: .75rem 1rem 0 .5rem;'">
                    <b-form-checkbox class="ml-2" v-model="expiredViewChecked"  @change="getAssignments()" size="lg"  switch>
                        {{viewStatus}}
                    </b-form-checkbox>
                </div>
            </b-col>
        </b-row>

        <loading-spinner v-if= "!isAssignmentDataMounted" />

        <b-card v-else no-body style="width: 50rem; margin: 0 auto 8rem auto">                                        
            <b-card id="AssignmentError" no-body>
                <h2 v-if="assignmentError" class="mx-1 mt-2"><b-badge v-b-tooltip.hover :title="assignmentErrorMsgDesc"  variant="danger"> {{assignmentErrorMsg}} <b-icon class="ml-3" icon = x-square-fill @click="assignmentError = false" /></b-badge></h2>
            </b-card>

            <div v-if="selectedAssignmentType.name != 'CourtRoom' && hasPermissionToCreateManageTypes">
                <b-card  v-if="!addNewAssignmentForm">                
                    <b-button size="sm" variant="success" @click="addNewAssignment"> <b-icon icon="plus" /> Add </b-button>
                </b-card>

                <b-card v-else id="addAssignmentForm" class="my-3" :border-variant="addFormColor" style="border:2px solid" body-class="m-0 px-0 py-1">
                    <add-assignment-form :type="selectedAssignmentType.label" :sortOrder="sortIndex" :formData="{}" :isCreate="true" v-on:submit="saveAssignment" v-on:cancel="closeAssignmentForm" />              
                </b-card>
            </div>
            <b-card v-else class="my-3"/>

            <div>
                <b-card no-body border-variant="white" bg-variant="white" v-if="!assignmentList.length" style="width: 50rem; margin: 0 auto 8rem auto">
                    <span class="text-muted ml-4 my-5">No {{selectedAssignmentType.label}} exists.</span>
                </b-card>

                <b-card v-else  no-body border-variant="light" bg-variant="white" style="width: 50rem; margin: 0 auto 8rem auto">
                    <b-table
                        :key="updateTable"
                        :items="assignmentList"
                        :fields="fields"                        
                        sort-icon-left
                        head-row-variant="primary"
                        :striped="!expiredViewChecked"
                        borderless
                        small
                        v-sortAssignmentType
                        responsive="sm"> 

                            <template v-slot:table-colgroup>
                                <col style="width:4rem">
                                <col>
                                <col>
                                <col style="width:6rem">
                            </template>
                                              
                            <template v-slot:head(code) >
                                <span>{{selectedAssignmentType.label}}</span> 
                            </template>

                            <template v-slot:head(scope) = "data" >
                                <span v-if="selectedAssignmentType.name != 'CourtRoom'">{{data.label}}</span> 
                                <span v-else></span>
                            </template>

                            <template v-slot:cell(sortOrder)= "data" >                                
                                <span v-if="!data.item['_rowVariant']"><b-icon class="handle ml-3" icon="arrows-expand"/></span> 
                            </template>

                            <template v-slot:cell(edit)="data" >                                  
                                <b-button v-if="!data.item['_rowVariant']" 
                                    class="ml-2 px-1"
                                    style="padding: 1px 2px 1px 2px;" 
                                    size="sm" 
                                    v-b-tooltip.hover
                                    title="Expire" 
                                    variant="warning"
                                    :disabled="!hasPermissionToExpireManageTypes" 
                                    @click="confirmDeleteAssignment(data.item)">
                                    <b-icon icon="clock" 
                                        font-scale="1" 
                                        variant="white"/>
                                </b-button>
                                <b-button v-if="data.item['_rowVariant']" :disabled="!hasPermissionToExpireManageTypes" class="my-0 ml-2 py-0 px-1" size="sm" variant="warning" @click="confirmUnexpireAssignment(data.item)"><b-icon icon="arrow-counterclockwise" font-scale="1.25" variant="danger"/></b-button>
                                <b-button v-if="hasPermissionToEditManageTypes && selectedAssignmentType.name != 'CourtRoom'" :disabled="data.item['_rowVariant']?true:false" class="my-0 py-0" size="sm" variant="transparent" @click="editAssignment(data)"><b-icon icon="pencil-square" font-scale="1.25" variant="primary"/></b-button>
                            </template>

                            <template v-slot:row-details="data">
                                <b-card :id="'#Assignment-'+data.item.code" body-class="m-0 px-0 py-1" :border-variant="addFormColor" style="border:2px solid">
                                    <add-assignment-form :type="selectedAssignmentType.label" :sortOrder="data.item.sortOrder" :formData="data.item" :isCreate="false" v-on:submit="saveAssignment" v-on:cancel="closeAssignmentForm" />
                                </b-card>
                            </template>
                    </b-table>
                </b-card>                 
            </div>                                     
        </b-card>

        <b-modal v-model="confirmDelete" id="bv-modal-confirm-delete" header-class="bg-warning text-light">
            <template v-slot:modal-title>
                    <h2 v-if="deleteType == 'expire'" class="mb-0 text-light">Confirm Expire Assignment</h2>
                    <h2 v-else class="mb-0 text-light">Confirm Unexpire Assignment</h2>                     
            </template>
            <h4 v-if="deleteType == 'expire'">Are you sure you want to Expire the "{{selectedAssignmentType.label}}: {{assignmentToDelete.code?assignmentToDelete.code:''}}"  assignment type?</h4>
            <h4 v-else>Are you sure you want to Unexpire the "{{selectedAssignmentType.label}}: {{assignmentToDelete.code?assignmentToDelete.code:''}}"  assignment type?</h4>
            <template v-slot:modal-footer>
                <b-button variant="danger" @click="deleteAssignment()">Confirm</b-button>
                <b-button variant="primary" @click="$bvModal.hide('bv-modal-confirm-delete')">Cancel</b-button>
            </template>            
            <template v-slot:modal-header-close>                 
                <b-button variant="outline-warning" class="text-light closeButton" @click="$bvModal.hide('bv-modal-confirm-delete')"
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

    </b-card>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from 'vue-property-decorator';
    import { namespace } from 'vuex-class';    
    import moment from 'moment-timezone';
    import * as _ from 'underscore';

    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/ManageTypesInformation";
    const manageTypesState = namespace("ManageTypesInformation");

    import "@store/modules/AssignmentTypesInformation";
    const assignmentTypesState = namespace("AssignmentTypesInformation");

    import PageHeader from "@components/common/PageHeader.vue"; 
    import AddAssignmentForm from "../ManageTypes/AddAssignmentForm.vue";
    
    import {locationInfoType, userInfoType} from '@/types/common'; 
    import {assignmentTypeInfoType}  from '@/types/ManageTypes/index';

    import sortAssignmentType from './utils/sortAssignmentType';
    
    @Component({
        components: {
            PageHeader,
            AddAssignmentForm
        },
        directives: {
            sortAssignmentType
        }
    })
    export default class AssignmentTypes extends Vue {

        @assignmentTypesState.Action
        ClearAssignmentTypesCache!: () => void;

        @assignmentTypesState.Action
        FetchAssignmentTypes!: () => Promise<any[]>;

        @assignmentTypesState.Getter
        getTypes!: any[];

        @assignmentTypesState.Getter
        getTypeOptions!: { name: string; code: number; label: string }[];

        @commonState.State
        public location!: locationInfoType;        @commonState.State
        public userDetails!: userInfoType;

        @manageTypesState.State
        public sortingAssignmentInfo!: {prvIndex: number; newIndex: number};

        hasPermissionToEditManageTypes = false;
        hasPermissionToCreateManageTypes = false;
        hasPermissionToExpireManageTypes = false;

        sectionHeader = 'Assignments';
        isAssignmentDataMounted = false;
        isEditOpen = false;
        latestEditData;

        assignmentError = false;
        assignmentErrorMsg = '';
        assignmentErrorMsgDesc = '';

        addNewAssignmentForm = false;
        addFormColor = 'dark';
        updateTable =0;

        sortIndex = 0;
        expiredViewChecked = false

        errorText=''
		openErrorModal=false;

        confirmDelete = false;
        deleteType = 'expire'; // 'unexpire'
        assignmentToDelete = {} as assignmentTypeInfoType;

        saveOrderFlag = false;

        assignmentList: assignmentTypeInfoType[] = [];
        //todo:
        assignmentTypeTabs: { name: string; code: number; label: string }[] = [];
        selectedAssignmentType: { name: string; code: number; label: string } = {name: 'CourtRoom', code: 0, label: 'Court Room'};
        previousSelectedAssignmentType: { name: string; code: number; label: string } = {name: 'CourtRoom', code: 0, label: 'Court Room'};
        fields =  
        [     
            {key:'sortOrder', label:'', sortable:false, tdClass: 'border-top' },       
            {key:'code', label:'', sortable:false, tdClass: 'border-top'  },
            {key:'scope',   label:'Location Specification',  sortable:false, tdClass: 'border-top', thClass:'',},  
            {key:'edit', label:'',  sortable:false, tdClass: 'border-top', thClass:'',},       
        ];

        @Watch('sortingAssignmentInfo', { immediate: true })
        sortingAssignmentInfoChange()
        {
            if (this.isAssignmentDataMounted) {
                for(const listIndex in this.assignmentList)
                    if(Number(listIndex) == this.sortingAssignmentInfo.prvIndex)
                        this.assignmentList[listIndex].sortOrder = this.sortingAssignmentInfo.newIndex*2 + Math.sign(this.sortingAssignmentInfo.newIndex-this.sortingAssignmentInfo.prvIndex)
                    else
                        this.assignmentList[listIndex].sortOrder *=2
                this.saveOrderFlag = true;
                this.refineSortOrders();
                
            }         
        } 
        
        @Watch('location.id', { immediate: true })
        locationChange()
        {
            if (this.isAssignmentDataMounted) {
                this.getAssignments()
            }            
        } 

        mounted () 
        {  
            this.fetchAssignmentTypeTabs().then(() => {
                this.getAssignmentTypesPermissions();
                this.getAssignments();
            });       
        }

        public getAssignmentTypesPermissions() {
            this.hasPermissionToEditManageTypes = this.userDetails.permissions.includes("EditTypes");
            this.hasPermissionToExpireManageTypes = this.userDetails.permissions.includes("ExpireTypes");
            this.hasPermissionToCreateManageTypes = this.userDetails.permissions.includes("CreateTypes");
        }

        public getAssignments() {
            this.closeAssignmentForm()
            Vue.nextTick(()=>{            
                this.isAssignmentDataMounted = false;
                const url = 'api/managetypes?codeType='+this.selectedAssignmentType.name +'&locationId='+this.location.id+'&showExpired='+this.expiredViewChecked;
                this.$http.get(url)
                    .then(response => {
                        if(response.data){
                            //console.log(response.data)
                            this.extractAssignments(response.data);                        
                        }
                        this.isAssignmentDataMounted = true;
                    },err => {
                        this.errorText=err.response.statusText+' '+err.response.status + '  - ' + moment().format(); 
                        if (err.response.status != '401') {
                            this.openErrorModal=true;
                        }  
                        this.isAssignmentDataMounted=true;
                    }) 
            });       
        }

        public async fetchAssignmentTypeTabs() {
            try {
                await this.ClearAssignmentTypesCache();
                await this.FetchAssignmentTypes();
                this.assignmentTypeTabs = this.getTypes.map((item: any) => ({
                    name: item.name,
                    code: item.code,
                    label: item.description
                }));
                // Optionally set default selectedAssignmentType
                if (this.assignmentTypeTabs.length > 0) {
                    this.selectedAssignmentType = this.assignmentTypeTabs[0];
                    this.previousSelectedAssignmentType = this.assignmentTypeTabs[0];
                }
            } catch (err) {
                this.errorText = `Error fetching assignment types`;
                this.openErrorModal = true;
            }
        }

        public extractAssignments(assignmentsJson) {

            this.assignmentList = [];
            this.sortIndex = assignmentsJson.length? 5000+assignmentsJson.length : 0;
            //console.log(this.sortIndex)
            for(const assignmentJson of assignmentsJson) {

                const assignment = {} as assignmentTypeInfoType;
                assignment.id = assignmentJson.id;
                assignment.code = assignmentJson.code;
                assignment.locationId = assignmentJson.locationId;
                if(this.selectedAssignmentType.name != 'CourtRoom') assignment['scope'] = assignmentJson.locationId? this.location.name : 'Province' 
                assignment['_rowVariant'] = '';
                let sortOrderOffset = 0;
                if(assignmentJson.expiryDate){
                    assignment['_rowVariant'] = 'info';
                    sortOrderOffset = 10000;
                }
                assignment.sortOrder = assignmentJson.sortOrderForLocation? assignmentJson.sortOrderForLocation.sortOrder :(sortOrderOffset+this.sortIndex++);
                //console.log(assignment.code+' '+assignment.sortOrder)
                assignment.type = assignmentJson.type; 
                this.assignmentList.push(assignment)                
            }

            //console.log(this.assignmentList)
            this.refineSortOrders();
        }

        public refineSortOrders(){
            this.assignmentList = _.sortBy(this.assignmentList,'sortOrder');
            for(const listIndex in this.assignmentList){
                if(!this.assignmentList[listIndex]['_rowVariant'])
                    this.assignmentList[listIndex].sortOrder = Number(listIndex);
            }
                
            this.updateTable++;
            //console.log(this.assignmentList)
            if(this.saveOrderFlag) this.saveSortOrders();           
        }

        public saveSortOrders(){
            this.saveOrderFlag = false;
            const sortOrders: {lookupCodeId: number ; sortOrder: number}[] = [];

            for(const assignment of this.assignmentList ){
                if(!assignment['_rowVariant'])
                    sortOrders.push({lookupCodeId: assignment.id , sortOrder: assignment.sortOrder})
            }
                
            const body = {
                sortOrderLocationId: this.location.id,
                sortOrders: sortOrders
            }

            //console.log(sortOrders)
            //console.log(body)
            const url = 'api/managetypes/updatesort' 

            this.$http.put(url, body)
                .then(response => {
                    console.log(response)
                }, err=>{
                    const errMsg = err.response.data.error;
                    this.assignmentErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.assignmentErrorMsgDesc = errMsg;
                    this.assignmentError = true;
                    location.href = '#AssignmentError'
                });
        }
    
        public addNewAssignment(){
            if(this.isEditOpen){
                location.href = '#Assignment-'+this.latestEditData.item.code
                this.addFormColor = 'danger'
            }else{
                this.addNewAssignmentForm = true;
                this.$nextTick(()=>{location.href = '#addAssignmentForm';})
            }
        }

        public confirmUnexpireAssignment(assignment){
            this.assignmentToDelete = assignment;           
            this.deleteType = 'unexpire';           
            this.confirmDelete = true; 
            //console.log(assignment)
        }

        public confirmDeleteAssignment(assignment){
            this.assignmentToDelete = assignment;
            this.deleteType = 'expire';           
            this.confirmDelete = true; 
            //console.log(assignment)
        }

        public deleteAssignment(){
            this.confirmDelete = false; 
            const url = 'api/managetypes/'+this.assignmentToDelete.id+'/'+this.deleteType;
            this.$http.put(url)
                .then(response => {
                    //console.log(response);
                    this.saveOrderFlag = true;
                    this.ClearAssignmentTypesCache(); // Clear cache when assignment is deleted
                    this.getAssignments();
                    
                }, err=>{
                    const errMsg = err.response.data.error;
                    this.assignmentErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.assignmentErrorMsgDesc = errMsg;
                    this.assignmentError = true;
                    location.href = '#AssignmentError'
                });
        }

        public editAssignment(assignment){
            //console.log(assignment)
            if(this.addNewAssignmentForm){
                location.href = '#addAssignmentForm'
                this.addFormColor = 'danger'
            }else if(this.isEditOpen){
                location.href = '#Assignment-'+this.latestEditData.item.code
                this.addFormColor = 'danger'               
            }else if(!this.isEditOpen && !assignment.detailsShowing){
                assignment.toggleDetails();
                this.isEditOpen = true;
                this.latestEditData = assignment
                Vue.nextTick().then(()=>{
                    location.href = '#Assignment-'+this.latestEditData.item.code
                });
            }  
        }

        public saveAssignment(body, iscreate){
            this.assignmentError = false;
            //console.log(body) 
            body['type']= this.selectedAssignmentType.code;
            body['description'] = body.code;
            const method = iscreate? 'post' :'put';            
            const url = 'api/managetypes'  
            const options = { method: method, url:url, data:body}
            
            this.$http(options)
                .then(response => {
                    this.ClearAssignmentTypesCache(); // Clear cache when assignment is saved
                    if(iscreate) 
                        this.addAssignmentToList(response.data);
                    else
                        this.modifyAssignmentList(response.data);
                    
                    this.closeAssignmentForm();
                }, err=>{
                    const errMsg = err.response.data.error;
                    this.assignmentErrorMsg = errMsg.slice(0,60) + (errMsg.length>60?' ...':'');
                    this.assignmentErrorMsgDesc = errMsg;
                    this.assignmentError = true;
                    location.href = '#AssignmentError'
                });                
        }

        public closeAssignmentForm() {                     
            this.addNewAssignmentForm = false; 
            this.addFormColor = 'secondary'
            if(this.isEditOpen){
                this.latestEditData.toggleDetails();
                this.isEditOpen = false;
            } 
        }

        public addAssignmentToList(assignmentJson){

            //console.log(assignmentJson)

            this.sortIndex++;
            const assignment = {} as assignmentTypeInfoType;
            assignment.id = assignmentJson.id;
            assignment.code = assignmentJson.code;
            assignment.locationId = assignmentJson.locationId;
            assignment['scope'] = assignmentJson.locationId? this.location.name : 'Province' 
            assignment.sortOrder = assignmentJson.sortOrderForLocation? assignmentJson.sortOrderForLocation.sortOrder :(this.sortIndex++);
            assignment.type = assignmentJson.type; 
            this.assignmentList.push(assignment);
            this.saveOrderFlag = true;
            this.refineSortOrders();
        }

        public modifyAssignmentList(modifiedAssignmentJson){            

            const index = this.assignmentList.findIndex(assignment =>{ if(assignment.id == modifiedAssignmentJson.id) return true})
            if(index>=0){
                this.assignmentList[index].id =  modifiedAssignmentJson.id;                
                this.assignmentList[index].code = modifiedAssignmentJson.code;                                
                this.assignmentList[index].locationId = modifiedAssignmentJson.locationId;
                this.assignmentList[index]['scope'] = modifiedAssignmentJson.locationId? this.location.name : 'Province';
                this.assignmentList[index].sortOrder = modifiedAssignmentJson.sortOrderForLocation? modifiedAssignmentJson.sortOrderForLocation.sortOrder :(this.sortIndex++);
                this.assignmentList[index].type = modifiedAssignmentJson.type;                
            }
            this.saveOrderFlag = true;
            this.refineSortOrders();            
        }

        get viewStatus() {
            if(this.expiredViewChecked) return 'All Assignments';else return 'Active Assignments'
        }
        
        public changeAssignment(){
            if(this.addNewAssignmentForm){
                location.href = '#addAssignmentForm'
                this.addFormColor = 'danger';
                this.selectedAssignmentType = this.previousSelectedAssignmentType;
            }else if(this.isEditOpen){
                location.href = '#Assignment-'+this.latestEditData.item.code
                this.addFormColor = 'danger'
                this.selectedAssignmentType =  this.previousSelectedAssignmentType;              
            }else{
                this.previousSelectedAssignmentType = this.selectedAssignmentType;
                this.expiredViewChecked = false;
                this.getAssignments();
            }
        }

    
}
</script>

<style scoped>   
    .card {
        border: white;
    }
</style>