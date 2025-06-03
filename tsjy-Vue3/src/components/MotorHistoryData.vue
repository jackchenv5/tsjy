<script setup>
import { vElementSize } from '@vueuse/components'
import { nextTick, onMounted, ref, toValue, watch } from 'vue'
import * as echarts from 'echarts'
import { useCssVar, useDark } from '@vueuse/core'
import { getMotorHistoryDataApi } from '@/apis/sawingMotor.js'
import { dayjs } from 'element-plus'

// region 组件
const model = defineModel()
const props = defineProps(['motorId', 'startTime', 'endTime'])
onMounted(async () => {
  await nextTick(async () => {
    initChart()
    updateChartColor()
  })
})
watch(model, async (value) => {
  if (value) {
    await getMotorHistoryData()
    model.value = false
  }
})
// endregion

// const {data} = await getMotorHistoryDataApi(
//       props.motorId,
//       dayjs(props.startTime).unix(),
//       dayjs(props.endTime).unix(),
//     )
//     console.log(data)

const emit = defineEmits(['data-loaded']);  //声明事件，该事件用于将电机数据反馈给父组件使用

// region 电机数据
const loadingMotorHistoryData = ref(false)
const motorHistoryData = ref({
  vibrationData: [],
  tensionData: [],
  followErrorData: [],
  temperatureErrorData: [],
  currentErrorData: [],
})
const getMotorHistoryData = async () => {
  if (loadingMotorHistoryData.value) {
    return
  }
  loadingMotorHistoryData.value = true
  try {
    const { data } = await getMotorHistoryDataApi(
      props.motorId,
      dayjs(props.startTime).unix(),
      dayjs(props.endTime).unix(),
    )
    motorHistoryData.value = data
    // console.log(motorHistoryData.value,data)
    updateChartData();
    //触发事件，将加载好的数据传递给父组件
    emit('data-loaded',motorHistoryData.value);  //使用emit触发事件
  } catch (e) {
    console.error(e).value
  } finally {
    loadingMotorHistoryData.value = false
  }
}
// endregion

// region 图表
let chart
const chartRef = ref()
let chartOptions = {
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
    bottom: '70',
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
      yAxisIndex: 0,
      showSymbol: false,
      data: [],
      itemStyle: {
        color: '',
      },
      symbol: 'none',
      connectNulls: false,
      sampling: 'lttb',
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
      connectNulls: false,
      sampling: 'lttb',
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
      connectNulls: false,
      sampling: 'lttb',
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
      connectNulls: false,
      sampling: 'lttb',
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
      connectNulls: false,
      sampling: 'lttb',
    },
  ],
  // 历史数据显示中下方的滑动条，默认初始显示前20%数据，可以调整
  dataZoom: [
    {
      type: 'inside',
      start: 0,
      end: 20,
    },
    {
      start: 0,
      end: 20,
    },
  ],
}
const initChart = () => {
  chart = echarts.init(chartRef.value)
}
const updateChart = () => {
  if (chart) {
    chart.setOption(chartOptions, true)
  }
}
//设置图例的颜色
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

//遍历振动、张力、跟随误差、温度和电流数据的值，并显示在图表当中
const updateChartData = () => {
  let vibrationData = []
  let tensionData = []
  let followErrorData = []
  let temperatureErrorData = []
  let currentErrorData = []

  for (let i = 0; i < motorHistoryData.value.vibrationData.length; i++) {
    const data = motorHistoryData.value.vibrationData[i]
    vibrationData.push({
      time: data.time.toString(),
      value: [dayjs.unix(data.time).toDate(), data.value],
    })
  }
  for (let i = 0; i < motorHistoryData.value.tensionData.length; i++) {
    const data = motorHistoryData.value.tensionData[i]
    tensionData.push({
      time: data.time.toString(),
      value: [dayjs.unix(data.time).toDate(), data.value],
    })
  }
  for (let i = 0; i < motorHistoryData.value.followErrorData.length; i++) {
    const data = motorHistoryData.value.followErrorData[i]
    followErrorData.push({
      time: data.time.toString(),
      value: [dayjs.unix(data.time).toDate(), data.value],
    })
  }
  for (let i = 0; i < motorHistoryData.value.temperatureErrorData.length; i++) {
    const data = motorHistoryData.value.temperatureErrorData[i]
    temperatureErrorData.push({
      time: data.time.toString(),
      value: [dayjs.unix(data.time).toDate(), data.value],
    })
  }
  for (let i = 0; i < motorHistoryData.value.currentErrorData.length; i++) {
    const data = motorHistoryData.value.currentErrorData[i]
    currentErrorData.push({
      time: data.time.toString(),
      value: [dayjs.unix(data.time).toDate(), data.value],
    })
  }

  chartOptions.series[0].data = vibrationData
  chartOptions.series[1].data = tensionData
  chartOptions.series[2].data = followErrorData
  chartOptions.series[3].data = temperatureErrorData
  chartOptions.series[4].data = currentErrorData
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
  <div class="chart" ref="chartRef" v-element-size="onResize"></div>
</template>

<style scoped lang="scss">
.chart {
  flex: 1;
  overflow: hidden;
}
</style>
