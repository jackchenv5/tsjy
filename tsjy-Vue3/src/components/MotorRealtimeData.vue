<script setup>
import { nextTick, onBeforeUnmount, onMounted, ref, toValue, watch } from 'vue'
import { getMotorDataApi } from '@/apis/sawingMotor.js'
import { getMotorBindingApi } from '@/apis/sawingMotor.js'
import * as echarts from 'echarts'
import { useCssVar, useDark } from '@vueuse/core'
import { vElementSize } from '@vueuse/components'
import {ElForm} from 'element-plus'
import AlarmParameterForm from './AlarmParameterForm.vue'

// region 组件
const props = defineProps(['motorId','motor'])
onMounted(async () => {
  //
  await getMotorBindingData()
  //
  // console.log(props.motorId,props.motor)
  await getMotorData()
  startInterval()
  await nextTick(async () => {
    initChart()
    updateChartColor()
  })
})
onBeforeUnmount(() => {
  stopInterval()
})
watch(
  () => props.motorId,
  //
  () => props.motor,
  async (newVal) => {
    if (newVal) {
      await getMotorBindingData()
    }
  },
  //
  () => {
    motorData.value = {
      vibration: 0,
      tension: 0,
      followError: 0,
      temperature: 0,
      current:0,
    }
    vibration = []
    tension = []
    followError = []
    temperature = []
    current = []
    updateChartData()
  },
)
// endregion

// region 定时获取电机数据
let intervalId
const startInterval = () => {
  stopInterval()
  intervalId = setInterval(getMotorData, 1000)
}
const stopInterval = () => {
  if (intervalId) {
    clearInterval(intervalId)
  }
}
//
const loadingGetMotorBinding = ref(false)
const motorBindingData = ref()
const getMotorBindingData = async () => {
  if (loadingGetMotorBinding.value) {
    return
  }
  loadingGetMotorBinding.value = true
  try {
    const { data } = await getMotorBindingApi(props.motor['id'])
    motorBindingData.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingGetMotorBinding.value = false
  }
}
//
// endregion

// region 电机数据
const loadingGetMotorData = ref(false)
const motorData = ref({
  vibration: 0,
  tension: 0,
  followError: 0,
  temperature: 0,
  current:0,
})
const getMotorData = async () => {
  if (loadingGetMotorData.value) {
    return
  }
  loadingGetMotorData.value = true
  try {
    const { data } = await getMotorDataApi(props.motorId)
    motorData.value = data
    updateChartData()
  } catch (e) {
    console.error(e)
  } finally {
    loadingGetMotorData.value = false
  }
}
// endregion

//报警阈值设定弹窗
const dialogVisible = ref(false);  //设置弹窗可见性
const alarmParameters = ref(
  {       
    bindingType: 0,
    connectionName: '',
    connectorInstance: '',
    dataPoint: '',
    id: 0,
    max: 0,
    maxAlarm: 0,
    maxWarning: 0,
    min: 0,
    minAlarm: 0,
    minWarning: 0,
    motorId: 0,
    name: ''
  }
)

const openAlarmSettingDialog = () => {
  dialogVisible.value = true;
}

const handleClose = () =>{
  dialogVisible.value = false;
}

const handleAlarmParameterSubmit = (newParams) => {
  alarmParameters.value = newParams;
  // console.log('设定的阈值为：',alarmParameters.value,newParams);

  // 动态更新 chartOptions 中的 markLine 配置
  chartOptions.series[0].markLine.data = [
    { yAxis: alarmParameters.value.max, name: '正常上阈值', lineStyle: { type: 'solid', width: 3, color: 'green' } },
    { yAxis: alarmParameters.value.min, name: '正常下阈值', lineStyle: { type: 'solid', width: 3, color: 'green' } },
    { yAxis: alarmParameters.value.maxWarning, name: '预警上阈值', lineStyle: { type: 'solid', width: 3, color: 'orange' } },
    { yAxis: alarmParameters.value.minWarning, name: '预警下阈值', lineStyle: { type: 'solid', width: 3, color: 'orange' } },
    { yAxis: alarmParameters.value.maxAlarm, name: '报警上阈值', lineStyle: { type: 'solid', width: 3, color: 'red' } },
    { yAxis: alarmParameters.value.minAlarm, name: '报警下阈值', lineStyle: { type: 'solid', width: 3, color: 'red' } }
  ];

  // 重新设置 ECharts 的配置
  if (chart) {
    chart.setOption(chartOptions, true);
  }

  dialogVisible.value = false;
}

// region 图表
let chart
const chartRef = ref()
let chartOptions = {
  animation: false,//关闭动画，避免范围线一直刷新
  textStyle: {
    color: '',
  },
  color: [],
  tooltip: {
    trigger: 'axis',
    axisPointer: {
      animation: false,
    },
  },
  legend: {
    top: '20',
    textStyle: {
      color: 'inherit',
    },
  },
  grid: {
    left: '50',
    right: '50',
    top: '50',
    bottom: '50',
  },
  xAxis: {
    type: 'time',
    splitLine: {
      show: false,
    },
  },
  yAxis: [
    {
      type: 'value',
      name: '振动',
      position: 'left',
      offset:25,
      min:0,
      max:10,
      axisLine: {
        show: true,
        lineStyle: {
        color: toValue(useCssVar('--chart-color-1')), // 设置 Y 轴轴线颜色
      },
      },
      axisLabel: {
        showMinLabel: true,
        textStyle: {
        color: toValue(useCssVar('--chart-color-1')), // 设置 Y 轴刻度标签颜色
      },
      },
      nameTextStyle: {
      color: toValue(useCssVar('--chart-color-1')), // 轴名称颜色
      },
      alignTicks: true,
      boundaryGap: [0, '20%'],
    },
    {
      type: 'value',
      name: '张力',
      offset:-5,
      position: 'left',
      min:0,
      max:100,
      axisLine: {
        show: true,
        lineStyle: {
        color: toValue(useCssVar('--chart-color-2')), // 设置 Y 轴轴线颜色
      },
      },
      axisLabel: {
        showMinLabel: true,
        textStyle: {
        color: toValue(useCssVar('--chart-color-2')), // 设置 Y 轴刻度标签颜色
      },
      },
      nameTextStyle: {
      color: toValue(useCssVar('--chart-color-2')), // 轴名称颜色
      },
      alignTicks: true,
      boundaryGap: [0, '20%'],
    },
    {
      type: 'value',
      name: '跟随误差',
      position: 'right',
      offset:-40,
      min:0,
      max:10,
      axisLine: {
        show: true,
        lineStyle: {
        color: toValue(useCssVar('--chart-color-3')), // 设置 Y 轴轴线颜色
      },
      },
      axisLabel: {
        showMinLabel: true,
        textStyle: {
        color: toValue(useCssVar('--chart-color-3')), // 设置 Y 轴刻度标签颜色
      },
      },
      nameTextStyle: {
      color: toValue(useCssVar('--chart-color-3')), // 轴名称颜色
      },
      alignTicks: true,
      boundaryGap: [0, '20%'],
    },
    {
      type: 'value',
      name: '温度',
      position: 'right',
      offset:-2,
      min:0,
      max:100,
      axisLine: {
        show: true,
        lineStyle: {
        color: toValue(useCssVar('--chart-color-4')), // 设置 Y 轴轴线颜色
      },
      },
      axisLabel: {
        showMinLabel: true,
        textStyle: {
        color: toValue(useCssVar('--chart-color-4')), // 设置 Y 轴刻度标签颜色
      },
      },
      nameTextStyle: {
      color: toValue(useCssVar('--chart-color-4')), // 轴名称颜色
      },
      alignTicks: true,
      boundaryGap: [0, '20%'],
    },
    {
      type: 'value',
      name: '电流',
      position: 'right',
      offset : 28,
      min:0,
      max:50,
      axisLine: {
        show: true,
        lineStyle: {
        color: toValue(useCssVar('--chart-color-5')), // 设置 Y 轴轴线颜色
      },
      },
      axisLabel: {
        showMinLabel: true,
        textStyle: {
        color: toValue(useCssVar('--chart-color-5')), // 设置 Y 轴刻度标签颜色
      },
      },
      nameTextStyle: {
      color: toValue(useCssVar('--chart-color-5')), // 轴名称颜色
      },
      alignTicks: true,
      boundaryGap: [0, '20%'],
    },
  ],
  series: [
    {
      name: '振动',
      type: 'line',
      showSymbol: false,
      data: [],
      markLine:{
        symbol: ['none', 'none'],
        data:[
          {yAxis: 0,name:'正常上阈值',lineStyle:{ type:'solid',width:3,color : 'green'}},
          {yAxis: 0,name:'正常下阈值',lineStyle:{ type:'solid',width:3,color : 'green'}},
          {yAxis: 0,name:'预警上阈值',lineStyle:{ type:'solid',width:3,color : 'orange'}},
          {yAxis: 0,name:'预警下阈值',lineStyle:{ type:'solid',width:3,color : 'orange'}},
          {yAxis: 0,name:'报警上阈值',lineStyle:{ type:'solid',width:3,color : 'red'}},
          {yAxis: 0,name:'报警下阈值',lineStyle:{ type:'solid',width:3,color : 'red'}},
        ]
      },
      itemStyle: {
        color: '',
      },
      symbol: 'none',
    },
    {
      name: '张力',
      type: 'line',
      yAxisIndex: 1,
      showSymbol: false,
      data: [],
      itemStyle: {
        color: '',
      },
      symbol: 'none',
    },
    {
      name: '跟随误差',
      type: 'line',
      yAxisIndex: 2,
      showSymbol: false,
      data: [],
      itemStyle: {
        color: '',
      },
      symbol: 'none',
    },
    {
      name: '温度',
      type: 'line',
      yAxisIndex: 3,
      showSymbol: false,
      data: [],
      itemStyle: {
        color: '',
      },
      symbol: 'none',
    },
    {
      name: '电流',
      type: 'line',
      yAxisIndex: 4,
      showSymbol: false,
      data: [],
      itemStyle: {
        color: '',
      },
      symbol: 'none',
    },
  ],
}
const initChart = () => {
  chart = echarts.init(chartRef.value);
  chart.setOption(chartOptions);
}
const updateChart = () => {
  if (chart) {
    chart.setOption(chartOptions, true)
  }
}
const updateChartColor = () => {
  chartOptions.textStyle.color = toValue(useCssVar('--gray-9'))
  chartOptions.color = [
    toValue(useCssVar('--chart-color-1')),
    toValue(useCssVar('--chart-color-2')),
    toValue(useCssVar('--chart-color-3')),
    toValue(useCssVar('--chart-color-4')),
    toValue(useCssVar('--chart-color-5')),
  ]
  updateChart()
}
useDark({
  onChanged() {
    if (chart) {
      updateChartColor()
    }
  },
})
let vibration = []
let tension = []
let followError = []
let temperature = []
let current = []
const pointCount = ref(60 * 5)
const updateChartData = () => {
  if (vibration.length > pointCount.value) {
    vibration.shift()
  }
  if (tension.length > pointCount.value) {
    tension.shift()
  }
  if (followError.length > pointCount.value) {
    followError.shift()
  }
  if (temperature.length > pointCount.value) {
    temperature.shift()
  }
  if (current.length > pointCount.value) {
    current.shift()
  }

  let now = new Date()

  vibration.push({
    name: now.toString(),
    value: [new Date(), motorData.value.vibration],
  })
  tension.push({
    name: now.toString(),
    value: [new Date(), motorData.value.tension],
  })
  followError.push({
    name: now.toString(),
    value: [new Date(), motorData.value.followError],
  })
  temperature.push({
    name: now.toString(),
    value: [new Date(), motorData.value.temperature],
  })
  current.push({
    name: now.toString(),
    value: [new Date(), motorData.value.current],
  })
  
  chartOptions.series[0].data = vibration
  chartOptions.series[1].data = tension
  chartOptions.series[2].data = followError
  chartOptions.series[3].data = temperature
  chartOptions.series[4].data = current
  updateChart()
}
let timerId
const onResize = () => {
  if (timerId) {
    clearTimeout(timerId)
  }
  timerId = setTimeout(() => {
    if (chart) {
      chart.resize()
    }
  }, 50)
}
// endregion
</script>

<template>
  <div class="container">
    <div class="chart-container">
      <div class="chart-setting">
        <ElForm>
          <ElFormItem label="数据点数" style="width: 20rem">
              <ElInputNumber
                :min="60"
                :max="60 * 60"
                :step="10"
                v-model="pointCount"
              >
              </ElInputNumber>
          </ElFormItem>
        </ElForm>
        <h2>&nbsp;&nbsp;&nbsp;</h2>
          <ElButton type="primary" plain @click="openAlarmSettingDialog">报警阈值设定</ElButton>
          <ElDialog
            v-model="dialogVisible"
            title="报警阈值设定"
            width="20%"
            @close="handleClose"
          >
          <div class="centered-content">
            <AlarmParameterForm 
             @submit="handleAlarmParameterSubmit"
             :motor = 'motorBindingData'
             />
          </div>
          </ElDialog>
      </div>
      <div class="chart" ref="chartRef" v-element-size="onResize"></div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.container {
  flex: 1;
  display: flex;
  flex-flow: row;
  grid-gap: 1rem;
  overflow: auto;

  > .data {
    display: flex;
    flex-flow: column;
    grid-gap: 1rem;
    width: 16rem;
  }

  > .chart-container {
    flex: 1;
    display: flex;
    flex-flow: column;

    > .chart-setting {
      display: flex;

      > .centered-content {
        text-align: center; /* 水平居中 */
        margin: auto; /* 垂直居中 */
        height: 100%; /* 确保占满整个弹窗高度 */
      }
    }
    > .chart {
      flex: 1;
      overflow: hidden;
    }
  }
}
</style>
