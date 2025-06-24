import { RouteConfig } from 'vue-router';
import VueResource from 'vue-resource';
import Home from '@components/Home.vue';
import Logout from '@components/Logout.vue';
import RequestAccess from '@components/RequestAccess.vue';
import ManageDutyRoster from '@components/DutyRoster/ManageDutyRoster.vue';
import ViewDutyRoster from '@components/DutyRoster/ViewDutyRoster.vue';
import ManageAssignments from '@components/DutyRoster/manageAssignments/ManageAssignments.vue';
import ManageSchedule from '@components/ShiftSchedule/ManageSchedule.vue';
import DistributeSchedule from '@components/ShiftSchedule/DistributeSchedule.vue';
import MyTeamMembers from '@components/MyTeam/MyTeamMembers.vue';
import DefineRolesAccess from '@components/MyTeam/DefineRolesAccess.vue';
import ViewReports from '@components/MyTeam/ViewReports.vue';
import AssignmentTypes from '@components/ManageTypes/AssignmentTypes.vue';
import DefineTypes from '@components/ManageTypes/DefineTypes.vue';
import LeaveTrainingTypes from '@components/ManageTypes/LeaveTrainingTypes.vue';
import store from "@/store";

function dontDisplayHeader(to: any, from: any, next: any) {
	store.commit('CommonInformation/setDisplayHeader',false);
	next();
}

function displayFooter(to: any, from: any, next: any) {
	store.commit('CommonInformation/setDisplayHeader',true);
	store.commit('CommonInformation/setDisplayFooter',true);
	next();
}

function dontDisplayFooter(to: any, from: any, next: any) {
	store.commit('CommonInformation/setDisplayHeader',true);
	store.commit('CommonInformation/setDisplayFooter',false);
	next();
}

function waitFor(callback) {
	if(!store.state.CommonInformation.userDetails.homeLocationId) {
		window.setTimeout(waitFor.bind(null, callback), 100);
	} else {
		callback();
	}
}

async function checkPermission(to: any, from: any, next: any) {

	try {	
		await store.state.CommonInformation.userDetails;
		await waitFor(() => {
			const userPermissions = store.state.CommonInformation.userDetails.permissions;
			if(to.name == "ManageDutyRoster" || to.name == "ViewDutyRoster" || to.name == "ManageAssignments") {
				if (userPermissions.includes("ViewDutyRoster")){        
					dontDisplayFooter(to, from, next);	
				} else {
					next({ path: "/request-access" });
				}
			} else if(to.name == "MyTeamMembers") {
				if (userPermissions.includes("ViewProvince") || userPermissions.includes("ViewRegion") || userPermissions.includes("ViewHomeLocation") || userPermissions.includes("ViewAssignedLocation")){        
					displayFooter(to, from, next);	
				} else {
					next({ path: "/manage-duty-roster" });
				}
			} else if(to.name == "DefineRolesAccess") {
				if (userPermissions.includes(to.meta.requiredPermission)){
					displayFooter(to, from, next);	
				} else {
					next({ path: "/manage-duty-roster" });
				}
			} else {
				if (userPermissions.includes(to.meta.requiredPermission)){
					dontDisplayFooter(to, from, next);	
				} else {
					next({ path: "/manage-duty-roster" });
				}
			}
		})  

	} catch(e) {
		next({ path: "/manage-duty-roster" });
	}

}

const routes: Array<RouteConfig> = [

	{
		path: '/',
		name: 'Home',
		beforeEnter: displayFooter,
		component: Home
	},
	{
		path: '/logout',
		name: 'Logout',
		beforeEnter: dontDisplayHeader,
		component: Logout
	},	
	{
		path: '/request-access',
		name: 'RequestAccess',
		beforeEnter: dontDisplayHeader,
		component: RequestAccess
	},
	{
		path: '/manage-duty-roster',
		name: 'ManageDutyRoster',
		beforeEnter: checkPermission,
		component: ManageDutyRoster    
	},
	{
		path: '/view-duty-roster',
		name: 'ViewDutyRoster',
		beforeEnter: checkPermission,
		component: ViewDutyRoster    
	},
	{
		path: '/manage-assignments',
		name: 'ManageAssignments',
		beforeEnter: checkPermission,
		component: ManageAssignments    
	},
	{    
		path: '/manage-shift-schedule',
		name: 'ManageSchedule',
		beforeEnter: checkPermission,
		component: ManageSchedule,
		meta:{requiredPermission: 'ViewShifts'}
	},
	{
		path: '/distribute-shift-schedule',
		name: 'DistributeSchedule',
		beforeEnter: checkPermission,
		component: DistributeSchedule,
		meta:{requiredPermission: 'ViewDistributeSchedule'}  
	},
	{
		path: '/team-members',
		name: 'MyTeamMembers',
		beforeEnter: checkPermission,
		component: MyTeamMembers
	},
	{
		path: '/define-roles-access',
		name: 'DefineRolesAccess',
		beforeEnter: checkPermission,
		component: DefineRolesAccess,
		meta:{requiredPermission: 'ViewRoles'}  
	},
	{
		path: '/reports',
		name: 'ViewReports',
		beforeEnter: checkPermission,
		component: ViewReports,
		meta:{requiredPermission: 'GenerateReports'}  
	},
	{
		path: '/define-types',
		name: 'DefineTypes',
		beforeEnter: checkPermission,
		component: DefineTypes,
		meta: {requiredPermission: 'EditTypes'}
	},
	{    
		path: '/assignment-types',
		name: 'AssignmentTypes',
		beforeEnter: checkPermission,
		component: AssignmentTypes,
		meta: {requiredPermission: 'EditTypes'}
	},
	{
		path: '/leave-training-types',
		name: 'LeaveTrainingTypes',
		beforeEnter: checkPermission,
		component: LeaveTrainingTypes,
		meta: {requiredPermission: 'EditTypes'}  
	}
]

export default routes