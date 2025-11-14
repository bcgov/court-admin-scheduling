import Vue from 'vue';
import Vuex from 'vuex';
import CommonInformation from '@/store/modules/CommonInformation';
import TeamMemberInformation from '@/store/modules/TeamMemberInformation';
import ManageTypesInformation from '@/store/modules/ManageTypesInformation';
import ShiftScheduleInformation from '@/store/modules/ShiftScheduleInformation';
import DutyRosterInformation from '@/store/modules/DutyRosterInformation';
import AssignmentScheduleInformation from './modules/AssignmentScheduleInformation';
import AssignmentTypesInformation from './modules/AssignmentTypesInformation';
Vue.use(Vuex)

const store = new Vuex.Store({
	modules: {
		CommonInformation,
		TeamMemberInformation,
		ManageTypesInformation,
		ShiftScheduleInformation,
		DutyRosterInformation,
		AssignmentScheduleInformation,
		AssignmentTypesInformation
	}
})

export default store
