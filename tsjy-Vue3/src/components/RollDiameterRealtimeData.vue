<script setup>
import { nextTick, onBeforeUnmount, onMounted, ref, toValue, watch } from 'vue'
import { getMotorBindingApi } from '@/apis/sawingMotor.js'
// 只需要卷径数据接口
import { getRollDiameterDataApi } from '@/apis/sawingMotor.js'
import * as echarts from 'echarts'
import { useCssVar, useDark } from '@vueuse/core'
import { vElementSize } from '@vueuse/components'
import { ElForm } from 'element-plus'
import AlarmParameterForm from './AlarmParameterForm.vue'

// region 组件
const props = defineProps(['motorId', 'motor'])
onMounted(async () => {
  await getMotorBindingData()
  await getRollDiameterData() // 只获取卷径数据
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
  () => props.motor,
  async (newVal) => {
    if (newVal) {
      await getMotorBindingData()
    }
  },
  () => {
    rollDiameterData.value = {
      realRollDiameter: 0,
      reasonableRollDiameterMin: 0,
      reasonableRollDiameterMax: 0,
    }
    realRollDiameter = []
    reasonableRollDiameter = []
    updateChartData()
  },
)

// region 定时获取数据
let intervalId
const startInterval = () => {
  stopInterval()
  intervalId = setInterval(() => {
    getRollDiameterData() // 只获取卷径数据
  }, 1000)
}
const stopInterval = () => {
  if (intervalId) {
    clearInterval(intervalId)
  }
}
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

// region 卷径数据
const loadingGetRollDiameterData = ref(false)
const rollDiameterData = ref({
  realRollDiameter: 0,
  reasonableRollDiameterMin: 0,
  reasonableRollDiameterMax: 0,
})
const getRollDiameterData = async () => {
  if (loadingGetRollDiameterData.value) {
    return
  }
  loadingGetRollDiameterData.value = true
  try {
    const { data } = await getRollDiameterDataApi(props.motorId)
    rollDiameterData.value = data
    updateChartData()
  } catch (e) {
    console.error(e)
  } finally {
    loadingGetRollDiameterData.value = false
  }
}

// 报警阈值设定弹窗
const dialogVisible = ref(false)
const alarmParameters = ref({
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
})

const openAlarmSettingDialog = () => {
  dialogVisible.value = true
}

const handleClose = () => {
  dialogVisible.value = false
}

const handleAlarmParameterSubmit = (newParams) => {
  alarmParameters.value = newParams
  chartOptions.series[0].markLine.data = [
    { yAxis: alarmParameters.value.max, name: '正常上阈值', lineStyle: { type: 'solid', width: 3, color: 'green' } },
    { yAxis: alarmParameters.value.min, name: '正常下阈值', lineStyle: { type: 'solid', width: 3, color: 'green' } },
    { yAxis: alarmParameters.value.maxWarning, name: '预警上阈值', lineStyle: { type: 'solid', width: 3, color: 'orange' } },
    { yAxis: alarmParameters.value.minWarning, name: '预警下阈值', lineStyle: { type: 'solid', width: 3, color: 'orange' } },
    { yAxis: alarmParameters.value.maxAlarm, name: '报警上阈值', lineStyle: { type: 'solid', width: 3, color: 'red' } },
    { yAxis: alarmParameters.value.minAlarm, name: '报警下阈值', lineStyle: { type: 'solid', width: 3, color: 'red' } }
  ]

  if (chart) {
    chart.setOption(chartOptions, true)
  }

  dialogVisible.value = false
}

// region 图表
let chart
const chartRef = ref()
let chartOptions = {
  animation: false,
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
      name: '卷径值',
      position: 'left',
      min: 0,
      max: 100, // 根据实际卷径范围调整
      axisLine: {
        show: true,
        lineStyle: {
          color: toValue(useCssVar('--chart-color-1')),
        },
      },
      axisLabel: {
        showMinLabel: true,
        textStyle: {
          color: toValue(useCssVar('--chart-color-1')),
        },
      },
      nameTextStyle: {
        color: toValue(useCssVar('--chart-color-1')),
      },
      alignTicks: true,
      boundaryGap: [0, '20%'],
    },
  ],
  series: [
    {
      name: '真实卷径值',
      type: 'line',
      showSymbol: false,
      data: [],
      markLine: {
        symbol: ['none', 'none'],
        data: [
          { yAxis: 0, name: '正常上阈值', lineStyle: { type: 'solid', width: 3, color: 'green' } },
          { yAxis: 0, name: '正常下阈值', lineStyle: { type: 'solid', width: 3, color: 'green' } },
          { yAxis: 0, name: '预警上阈值', lineStyle: { type: 'solid', width: 3, color: 'orange' } },
          { yAxis: 0, name: '预警下阈值', lineStyle: { type: 'solid', width: 3, color: 'orange' } },
          { yAxis: 0, name: '报警上阈值', lineStyle: { type: 'solid', width: 3, color: 'red' } },
          { yAxis: 0, name: '报警下阈值', lineStyle: { type: 'solid', width: 3, color: 'red' } },
        ],
      },
      itemStyle: {
        color: toValue(useCssVar('--chart-color-1')),
      },
      symbol: 'none',
    },
    {
      name: '卷径合理区间值',
      type: 'line',
      showSymbol: false,
      data: [],
      markArea: {
        itemStyle: {
          color: 'rgba(0, 128, 255, 0.2)', // 蓝色半透明区域
        },
        data: [
          [
            { yAxis: 0, name: '合理区间下限' },
            { yAxis: 0, name: '合理区间上限' },
          ],
        ],
      },
      itemStyle: {
        color: 'transparent', // 隐藏线条，只显示区域
      },
      symbol: 'none',
    },
  ],
}
const initChart = () => {
  chart = echarts.init(chartRef.value)
  chart.setOption(chartOptions)
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
let realRollDiameter = []
let reasonableRollDiameter = []
const pointCount = ref(60 * 5)
const updateChartData = () => {
  if (realRollDiameter.length > pointCount.value) {
    realRollDiameter.shift()
  }
  if (reasonableRollDiameter.length > pointCount.value) {
    reasonableRollDiameter.shift()
  }

  let now = new Date()

  realRollDiameter.push({
    name: now.toString(),
    value: [new Date(), rollDiameterData.value.realRollDiameter],
  })

  reasonableRollDiameter.push({
    name: now.toString(),
    value: [new Date(), rollDiameterData.value.realRollDiameter], // 仅用于占位
  })

  // 更新markArea的上下限
  chartOptions.series[1].markArea.data = [
    [
      { yAxis: rollDiameterData.value.reasonableRollDiameterMin, name: '合理区间下限' },
      { yAxis: rollDiameterData.value.reasonableRollDiameterMax, name: '合理区间上限' },
    ],
  ]

  chartOptions.series[0].data = realRollDiameter
  chartOptions.series[1].data = reasonableRollDiameter
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
      <div class="data-display">
        <div class="data-item">
          <span class="label">真实卷径值:</span>
          <span class="value">{{ rollDiameterData.realRollDiameter }}</span>
        </div>
        <div class="data-item">
          <span class="label">卷径合理区间值:</span>
          <span class="value">{{ rollDiameterData.reasonableRollDiameterMin }} - {{ rollDiameterData.reasonableRollDiameterMax }}</span>
        </div>
      </div>
      <div class="chart-setting">
        <ElForm>
          <ElFormItem label="数据点数" style="width: 20rem">
            <ElInputNumber
              :min="60"
              :max="60 * 60"
              :step="10"
              v-model="pointCount"
            />
          </ElFormItem>
        </ElForm>
        <h2></h2>
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
              :motor="motorBindingData"
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

  > .chart-container {
    flex: 1;
    display: flex;
    flex-flow: column;

    > .data-display {
      display: flex;
      flex-flow: row;
      gap: 2rem;
      padding: 1rem;
      background: #f5f5f5;
      border-bottom: 1px solid #e8e8e8;

      .data-item {
        display: flex;
        align-items: center;

        .label {
          font-weight: bold;
          margin-right: 0.5rem;
        }

        .value {
          font-size: 1.2rem;
          color: #333;
        }
      }
    }

    > .chart-setting {
      display: flex;
    }

    > .chart {
      flex: 1;
      overflow: hidden;
    }
  }
}

.centered-content {
  text-align: center;
  margin: auto;
  height: 100%;
}
</style>