<script setup>
import { useResize } from '#/common/composables/resize'
import { dayjs, useLocale } from 'element-plus'
import { onMounted, ref, toValue, watch } from 'vue'
import duration from 'dayjs/plugin/duration'
import { vElementSize } from '@vueuse/components'
import * as echarts from 'echarts'
import { useCssVar, useDark } from '@vueuse/core'

// region 组件
const { breakPoint } = useResize()
const { t, lang } = useLocale()
const model = defineModel()
const props = defineProps({
  statusData: {
    type: Array,
    require: true,
  },
})
onMounted(() => {
  initChart()
  updateChart()
})
dayjs.extend(duration)
// endregion

// region 图表
const chartRef = ref()
let chart
const valueFormatter = (seconds) => {
  return dayjs.duration(seconds * 1000).format('HH:mm:ss')
}
const yAxisLabelFormatter = (value) => {
  return Number((value / 60).toFixed(1))
}
let chartOptions = {
  textStyle: {
    color: '',
  },
  color: [],
  tooltip: {
    trigger: 'axis',
  },
  grid: {
    left: '30',
    right: '50',
    top: '50',
    bottom: '10',
    containLabel: true,
  },
  xAxis: {
    type: 'category',
    data: ['班组 1', '班组 2', '班组 3'],
    axisPointer: {
      type: 'shadow',
    },
    name: '班组',
    nameLocation: 'end',
  },
  yAxis: {
    type: 'value',
    name: '停机时长/分',
    min: 0,
    alignTicks: true,
    axisLine: {
      show: true,
    },
    axisLabel: {
      showMinLabel: true,
      formatter: yAxisLabelFormatter,
    },
  },
  series: [
    {
      name: '待机',
      type: 'bar',
      stack: 'total',
      barWidth: '20%',
      data: [120, 200, 150],
      itemStyle: {
        color: '',
      },
      tooltip: {
        valueFormatter: valueFormatter,
      },
    },
    {
      name: '停机',
      type: 'bar',
      stack: 'total',
      barWidth: '20%',
      data: [120, 200, 150],
      itemStyle: {
        color: '',
      },
      tooltip: {
        valueFormatter: valueFormatter,
      },
    },
    {
      name: '故障',
      type: 'bar',
      stack: 'total',
      barWidth: '20%',
      data: [120, 200, 150],
      itemStyle: {
        color: '',
      },
      tooltip: {
        valueFormatter: valueFormatter,
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
const updateChartData = (shiftNames, standbyData, stoppedData, errorData) => {
  chartOptions.xAxis.data = shiftNames
  chartOptions.series[0].data = standbyData
  chartOptions.series[1].data = stoppedData
  chartOptions.series[2].data = errorData

  updateChart()
}
const onResize = () => {
  chart.resize()
}
const updateChartLanguage = () => {
  chartOptions.xAxis.name = t('el.facilityStatus.stoppedData.shift')
  chartOptions.yAxis.name =
    `${t('el.facilityStatus.stoppedData.downtime')}` +
    `(${t('el.facilityStatus.stoppedData.minutes')})`
  chartOptions.series[0].name = t('el.facilityStatus.stoppedData.standby')
  chartOptions.series[1].name = t('el.facilityStatus.stoppedData.stopped')
  chartOptions.series[2].name = t('el.facilityStatus.stoppedData.error')

  updateChart()
}
watch(
  lang,
  () => {
    updateChartLanguage()
  },
  {
    immediate: true,
  },
)
// endregion

// region 数据
const loading = ref(false)
const processStatus = () => {
  let shifts = []
  for (const record of props.statusData) {
    const shift = record['shift']
    const index = shifts.findIndex((item) => item['id'] === shift['id'])
    if (index < 0) {
      shifts.push({
        id: shift['id'],
        name: shift['name'],
      })
    }
  }
  // 将 shifts 按照里面对象的 id 进行排序
  shifts.sort((a, b) => a['id'] - b['id'])
  const shiftNames = shifts.map((item) => item['name'])
  const standbyData = []
  const stoppedData = []
  const errorData = []
  for (const shift of shifts) {
    let standbySeconds = 0
    let stoppedSeconds = 0
    let errorSeconds = 0
    for (const record of props.statusData) {
      if (record['shift']['id'] === shift['id']) {
        if (record['status'] === 'Standby') {
          standbySeconds += record['duration']
        }
        if (record['status'] === 'Stopped') {
          stoppedSeconds += record['duration']
        }
        if (record['status'] === 'Error') {
          errorSeconds += record['duration']
        }
      }
    }
    standbyData.push(standbySeconds)
    stoppedData.push(stoppedSeconds)
    errorData.push(errorSeconds)
  }
  updateChartData(shiftNames, standbyData, stoppedData, errorData)
}
watch(model, async (value) => {
  if (value) {
    processStatus()
  }
  model.value = false
})
// endregion
</script>

<template>
  <div class="shift-compare-chart-root" :class="breakPoint">
    <div class="header">
      {{ t('el.facilityStatus.stoppedData.shiftCompare') }}
    </div>
    <div
      class="chart"
      ref="chartRef"
      v-loading="loading"
      v-element-size="onResize"
    ></div>
  </div>
</template>

<style scoped lang="scss">
.shift-compare-chart-root {
  flex: 1;
  display: flex;
  flex-flow: column;
  width: 0;
  border: 1px solid var(--el-border-color);
  border-radius: var(--el-border-radius-base);

  &.xs {
    width: auto;
  }

  > .header {
    background-color: var(--el-color-info-light-9);
    padding: 0.5rem 1rem;
    border-bottom: 1px solid var(--el-border-color);
    border-radius: var(--el-border-radius-base) var(--el-border-radius-base) 0 0;
  }

  > .chart {
    flex: 1 0;
    height: 0;
  }
}
</style>
