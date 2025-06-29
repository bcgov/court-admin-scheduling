<template>
    <div>
        
        <b-table
            :items="myTeamMembers" 
            :fields="gaugeFields"
            small
            striped
            head-row-variant="transparant"                    
            sort-by="availability"
            :sort-desc="true"
            :style="{ overflowX: 'scroll', height: getHeight, maxHeight: '100%', marginBottom: '-48px' }"                     
            :sticky-header="getHeight"                        
            borderless
            fixed>
                <template v-slot:table-colgroup> 
                    <col style="width:9rem">
                    <col>
                </template>

                <template v-slot:head(availability) >
                    <div class="gridfuel24" :style="{gridTemplateColumns: 'repeat(96, '+zoomLevel*8.333/100+'%)'}">
                        <div v-for="i in 24" :key="i" :style="{gridColumnStart: i,gridColumnEnd:(i+1), gridRow:'1/1'}">{{getBeautifyTime(i-1)}}</div>
                    </div>
                </template>

                <template v-slot:cell(availability)="data" >
                    <court-admin-availability-card class="m-0 p-0" :courtAdminInfo="data.item" :fullview="true" />
                </template>

                <template v-slot:head(name) > 
                    My Team                                                      
                </template>

                    <template v-slot:cell(name)="data" >
                    <div
                        :id="'gauge--'+data.item.courtAdmin.courtAdminId"                                                                                                 
                        style="height:2rem; font-size:14px; line-height: 1rem; text-transform: capitalize; margin:0; padding:0.5rem 0 0 0.25rem"
                        class="text-primary"
                        v-b-tooltip.hover.right                            
                        :title="data.item.fullName">
                            {{data.value}}
                    </div>
                </template>
        </b-table> 
        <div id="app-footer">
            <b-card 
                class="bg-light"
                header="Colours" 
                header-class=" m-0 p-0 bg-primary text-white text-center no-top-rounding" 
                no-body>
                <b-row style="margin:0 0 .25rem .25rem; width:7.6rem;">
                    <div
                        style="width:3.8rem;"
                        class="m-0 p-0"
                        v-for="color in dutyColors"
                        :key="color.colorCode">
                        <div :style="{backgroundColor:color.colorCode, width:'.65rem', height:'.65rem', borderRadius:'15px', margin:'.2rem .2rem 0 0', float:'left'}"/>
                        <div style="font-size:9px; text-transform: capitalize; margin:0 0 0 0; padding:0">{{color.name}}</div>
                    </div>
                </b-row>
            </b-card>
        </div>


        <b-modal size="xl" v-model="printCourtAdminFullview" footer-class="d-none" header-class="bg-primary text-light" title-class="h2" title="Print Duties">            
            
            <duty-pdf-view :myTeamMembers="myTeamMembers"/>

            <template v-slot:modal-header-close>                 
                <b-button variant="outline-white" class="text-light closeButton" @click="closePrint()"
                >&times;</b-button>
            </template>
        </b-modal>
    </div>
</template>

<script lang="ts">
    import { Component, Vue, Watch } from 'vue-property-decorator';
    import CourtAdminAvailabilityCard from './CourtAdminAvailabilityCard.vue'
    import { myTeamShiftInfoType, dutiesDetailInfoType} from '@/types/DutyRoster';
    import { userInfoType } from '@/types/common';

    import DutyPdfView from "./pdf/DutyPdfView.vue"

    import { namespace } from "vuex-class";   
    import "@store/modules/CommonInformation";
    const commonState = namespace("CommonInformation");

    import "@store/modules/DutyRosterInformation";   
    const dutyState = namespace("DutyRosterInformation");

    @Component({
        components: {
            CourtAdminAvailabilityCard,
            DutyPdfView
        }
    })
    export default class CourtAdminDayView extends Vue {
       
        @dutyState.State
        public shiftAvailabilityInfo!: myTeamShiftInfoType[];

   
        @dutyState.State
        public printCourtAdminFullview!: boolean;

        @dutyState.Action
        public UpdatePrintCourtAdminFullview!: (newPrintCourtAdminFullview: boolean) => void;
        
        @dutyState.State
        public zoomLevel!: number;

        @commonState.State
        public userDetails!: userInfoType;
        
        hasPermissionToAddAssignDuty = false;

        myTeamMembers: any[] = []

        gaugeFields = [
            {key:'name', label:'My Team', stickyColumn: true, thClass:'text-center text-white', tdClass:'border-bottom py-0 my-0', thStyle:'margin:0; padding:0;background-color:#556077;'},
            {key:'availability', label:'', thClass:'', tdClass:'p-0 m-0 bg-white', thStyle:'margin:0; padding:0;'},
        ]

        @Watch('shiftAvailabilityInfo')
        shiftAvailability() 
        {
            this.extractCourtAdminAvailability()
        }

        mounted()
        {
            this.fetchDutyColors();             
            this.hasPermissionToAddAssignDuty = this.userDetails.permissions.includes("CreateAndAssignDuties");
            this.extractCourtAdminAvailability() 
        }
        dutyColors: { name: string; colorCode: string }[] = [];
        // dutyColors = [
        //     {name:'court' , colorCode:'#189fd4'},
        //     {name:'jail' ,  colorCode:'#A22BB9'},
        //     {name:'transport', colorCode:'#ffb007'},
        //     {name:'other',  colorCode:'#7a4528'},
        //     {name:'overtime',colorCode:'#e85a0e'},
        //     {name:'free',   colorCode:'#e6d9e2'}            
        // ]
        
        fetchDutyColors() {
            const url = '/api/lookuptype/actives?category=Assignment';
            this.$http.get(url)
                .then((response: any) => {
                    this.dutyColors = response.data.map((item: any) => ({
                        name: item.description,
                        colorCode: item.displayColor
                    }));
                    // Optionally add static colors if needed
                    this.dutyColors.push({ name: 'overtime', colorCode: '#e85a0e' });
                    this.dutyColors.push({ name: 'free', colorCode: '#e6d9e2' });
               })
        }
        public extractCourtAdminAvailability(){
            this.myTeamMembers = [];
            for(const courtAdmin of this.shiftAvailabilityInfo){
                // console.log(courtAdmin.availability)
                //this.findIsland(courtAdmin.availability)
                this.myTeamMembers.push({                     
                    name: Vue.filter('truncate')(courtAdmin.lastName,10) + ', '+ courtAdmin.firstName.charAt(0).toUpperCase(),
                    fullName: courtAdmin.firstName + ' ' + courtAdmin.lastName,
                    availability: this.sumOfArrayElements(courtAdmin.availability),
                    courtAdmin: courtAdmin,
                    availabilityDetail: this.findAvailabilitySlots(courtAdmin.availability)
                })
            }
            // this.myTeamMembers=[...this.myTeamMembers, ...this.myTeamMembers,...this.myTeamMembers]
        }

        public findAvailabilitySlots(array){
            const availabilityDetail: dutiesDetailInfoType[] =[]
            const discontinuity = this.findDiscontinuity(array);
            const iterationNum = Math.floor((this.sumOfArrayElements(discontinuity) +1)/2);
            //console.log(iterationNum)
            for(let i=0; i< iterationNum; i++){
                const inx1 = discontinuity.indexOf(1)
                let inx2 = discontinuity.indexOf(2)
                discontinuity[inx1]=0
                if(inx2>=0) discontinuity[inx2]=0; else inx2=discontinuity.length 
                //console.error(inx1 + ' ' +inx2)
                availabilityDetail.push({
                        id: 10000+inx1 , 
                        startBin:inx1, 
                        endBin: inx2,
                        name: 'free' ,
                        colorCode: '#e6d9e2',
                        color: '#e6d9e2',
                        type: '',
                        code: ''
                    })
            }

            return availabilityDetail            
        }


        public findDiscontinuity(array){
            return array.map((arr,index)=>{
                if((index==0 && arr>0)||(arr>0 && array[index-1]==0)) return 1 ;
                else if(index>0 && arr==0 && array[index-1]>0) return 2 ;
                else return 0
            })
        }


        public getBeautifyTime(hour: number){
            return( hour>9? hour+':00': '0'+hour+':00')
        }

        public sumOfArrayElements(arrayA){
            return arrayA.reduce((acc, arr) => acc + (arr>0?1:0), 0)
        }


        get getHeight() {
            const windowHeight = Math.ceil(100*document.documentElement.clientHeight/this.zoomLevel);
            return windowHeight - this.calculateTableHeight() + 'px'
        }

        public calculateTableHeight() {
            const topHeaderHeight = (document.getElementsByClassName("app-header")[0] as HTMLElement)?.offsetHeight || 0;
            const secondHeader =  document.getElementById("dutyRosterNav")?.offsetHeight || 0;
            const footerHeight = 0//document.getElementById("footer")?.offsetHeight || 0;            
            const bottomHeight = footerHeight;
            //console.log(topHeaderHeight + bottomHeight + secondHeader)
            return (topHeaderHeight + bottomHeight + secondHeader)
        }

        public closePrint(){
            this.UpdatePrintCourtAdminFullview(false);
        }

    }
</script>

<style scoped>   

    .card {
        border: white;
    }

    .gridfuel24 {        
        display:grid;
        grid-template-columns: repeat(24, 8.333%);
        grid-template-rows: 1.57rem;
        inline-size: 100%;
        font-size: 9px;         
    }

    .gridfuel24 > * {      
        padding: .25rem 0;
        border: 1px dotted rgb(185, 143, 143);
        background-color: #003366;
        color: white;
        font-size: 12px;
    }
    #app-footer {
        padding: 2px 0px;
        position: absolute;
        position: fixed;
        right: 0;
        bottom: 0;
        z-index: 100;    
    }

</style>