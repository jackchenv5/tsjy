import { defineStore } from 'pinia';

export const useAlarmParametersStore = defineStore('alarmParameters', {
  state: () => ({
    parameters: {
      param_nomral_Max: 0,
      param_nomral_Min: 0,
      param_forewarning_Max: 0,
      param_forewarning_Min: 0,
      param_alarm_Max: 0,
      param_alarm_Min: 0
    }
  }),
  actions: {
    setParameters(newParams) {
      this.parameters = newParams;
    }
  }
});