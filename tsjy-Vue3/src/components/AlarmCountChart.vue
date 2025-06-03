<script setup>
import { inject, nextTick, onMounted, ref, toValue, watch } from 'vue'
import { getAlarmCountAsync } from '@/apis/sawingMachineAlarm.js'
import { dayjs } from 'element-plus'
import * as echarts from 'echarts'
import { useCssVar, useDark } from '@vueuse/core'
import { vElementSize } from '@vueuse/components'

// region 组件

const startTime = inject('startTime')
const endTime = inject('endTime')
onMounted(async () => {
  await nextTick(async () => {
    await getAlarmCount()
    initChart()
    updateChartColor()
  })
})
const model = defineModel()
watch(model, async (value) => {
  if (value) {
    await getAlarmCount()
    model.value = false
  }
})
// endregion

// region 报警统计数据
const alarmCountData = ref()
const loadingGetAlarmCount = ref(false)
const getAlarmCount = async () => {
  if (loadingGetAlarmCount.value) {
    return
  }
  loadingGetAlarmCount.value = true
  try {
    const param = {
      startTime: dayjs(startTime.value).unix(),
      endTime: dayjs(endTime.value).unix(),
    }
    const { data } = await getAlarmCountAsync(param)
    alarmCountData.value = data
    updateChartData()
  } catch (e) {
    console.error(e)
  } finally {
    loadingGetAlarmCount.value = false
  }
}
// endregion

// region chart
let chart
const chartRef = ref()
let chartOptions = {
  textStyle: {
    color: '',
  },
  color: [],
  tooltip: {
    trigger: 'axis',
    formatter: '{c} 次',
  },
  grid: {
    left: '50',
    right: '100',
    top: '50',
    bottom: '50',
  },
  xAxis: [
    {
      type: 'category',
      data: ['报警1', '报警2', '报警3', '报警4', '报警5', '其他'],
      axisPointer: {
        type: 'shadow',
      },
      name: '报警类型',
    },
  ],
  yAxis: [
    {
      type: 'value',
      name: '报警次数',
      min: 0,
      alignTicks: true,
      axisLine: {
        show: true,
      },
      axisLabel: {
        showMinLabel: true,
      },
    },
  ],
  series: [
    {
      name: '报警次数',
      type: 'bar',
      barWidth: '25%',
      data: [210, 200, 150, 120, 98, 195],
      itemStyle: {
        color: '',
      },
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
const updateChartColor = () => {
  chartOptions.textStyle.color = toValue(useCssVar('--gray-9'))
  chartOptions.color = [
    toValue(useCssVar('--primary-6')),
    toValue(useCssVar('--gold-6')),
    toValue(useCssVar('--red-6')),
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
const updateChartData = () => {
  let xData = []
  let yData = []
  for (let i = 0; i < alarmCountData.value.length; i++) {
    const message = alarmCountData.value[i]['message']
    if (message) {
      xData.push(alarmCountData.value[i]['message'])
    } else {
      xData.push('其他')
    }
    yData.push(alarmCountData.value[i]['count'])
  }
  chartOptions.xAxis[0].data = xData
  chartOptions.series[0].data = yData
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
    <div class="header">报警统计</div>
    <div class="chart" ref="chartRef" v-element-size="onResize"></div>
  </div>
</template>

<style scoped lang="scss">
.container {
  border: 1px solid var(--el-border-color);
  border-radius: var(--el-border-radius-base);
  flex: 1;
  display: flex;
  flex-flow: column;
  overflow: auto;

  > .header {
    display: flex;
    flex-flow: row;
    grid-gap: 1rem;
    align-items: center;
    background-color: var(--el-color-info-light-9);
    padding: 0.5rem 1rem;
    border-radius: var(--el-border-radius-base) var(--el-border-radius-base) 0 0;
    border-bottom: 1px solid var(--el-border-color);
  }

  > .chart {
    flex: 1;
    overflow: hidden;
  }
}
</style>
