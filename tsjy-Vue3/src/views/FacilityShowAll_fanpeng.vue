<template>
  <div id="app">
    <!-- 仪表盘和第一行信息 -->
    <div class="dashboard-info-row">
      <div ref="dashboard" style="width: 40%; height: 200px;"></div>
      <div class="info-group">
        <div class="info-row">
          <div class="info-item">
            <span>运行时间</span>
            <span class="number-display">{{ runningTime }} h</span>
          </div>
          <div class="info-item">
            <span>停机时间</span>
            <span class="number-display">{{ stopTime }} h</span>
          </div>
          <div class="info-item">
            <span>生产效率</span>
            <span class="number-display">{{ productionEfficiency }} %</span>
          </div>
          <div class="info-item">
            <span>柜内温度</span>
            <span class="number-display">{{ cabinetTemperature }} °C</span>
          </div>
          <div class="info-item">
            <span>车间温度</span>
            <span class="number-display">{{ workshopTemperature }} °C</span>
          </div>
        </div>
        <div class="info-row">
          <div class="info-item">
            <span>计划产量</span>
            <span class="number-display">{{ plannedOutput }}</span>
          </div>
          <div class="info-item">
            <span>实际产量</span>
            <span class="number-display">{{ actualOutput }}</span>
          </div>
          <div class="info-item">
            <span>完成效率</span>
            <span class="number-display">{{ completionEfficiency }} %</span>
          </div>
        </div>
      </div>
    </div>
    <!-- 中间区域模块 -->
    <div class="middle-modules">
      <div class="module">
        <h3>印刷组</h3>
        <div class="param-item">
          <span>温度</span>
          <span class="number-display">{{ printGroup.temperature }} °C</span>
        </div>
        <div class="param-item">
          <span>风速</span>
          <span class="number-display">{{ printGroup.windSpeed }} m/s</span>
        </div>
      </div>
      <div class="module">
        <h3>天桥一区</h3>
        <div class="param-item">
          <span>温度</span>
          <span class="number-display">{{ bridgeArea1.temperature }} °C</span>
        </div>
        <div class="param-item">
          <span>风速</span>
          <span class="number-display">{{ bridgeArea1.windSpeed }} m/s</span>
        </div>
      </div>
      <div class="module">
        <h3>天桥二区</h3>
        <div class="param-item">
          <span>温度</span>
          <span class="number-display">{{ bridgeArea2.temperature }} °C</span>
        </div>
        <div class="param-item">
          <span>风速</span>
          <span class="number-display">{{ bridgeArea2.windSpeed }} m/s</span>
        </div>
      </div>
      <div class="module">
        <h3>天桥三区</h3>
        <div class="param-item">
          <span>温度</span>
          <span class="number-display">{{ bridgeArea3.temperature }} °C</span>
        </div>
        <div class="param-item">
          <span>风速</span>
          <span class="number-display">{{ bridgeArea3.windSpeed }} m/s</span>
        </div>
      </div>
      <div class="module">
        <h3>浓度</h3>
        <div class="param-item">
          <span>浓度值</span>
          <span class="number-display">{{ concentration }} %</span>
        </div>
      </div>
    </div>
    <!-- 中间往下区域模块 -->
    <div class="lower-modules">
      <div class="module">
        <h3>放料相关</h3>
        <div class="param-item">
          <span>放牵张力</span>
          <span class="number-display">{{ unwindPullTension }}</span>
        </div>
        <div class="param-item">
          <span>放料张力</span>
          <span class="number-display">{{ unwindTension }}</span>
        </div>
        <div class="param-item">
          <span>转塔位置</span>
          <span class="number-display">{{ unwindTurretPosition }} A</span>
        </div>
        <div class="param-item">
          <span>卷径</span>
          <span class="number-display">{{ unwindDiameter }}</span>
        </div>
      </div>
      <div class="module">
        <h3>收料相关</h3>
        <div class="param-item">
          <span>收牵张力</span>
          <span class="number-display">{{ windPullTension }}</span>
        </div>
        <div class="param-item">
          <span>收料张力</span>
          <span class="number-display">{{ windTension }}</span>
        </div>
        <div class="param-item">
          <span>转塔位置</span>
          <span class="number-display">{{ windTurretPosition }} B</span>
        </div>
        <div class="param-item">
          <span>卷径</span>
          <span class="number-display">{{ windDiameter }}</span>
        </div>
      </div>
    </div>
    <!-- 底部图片 -->
    <div class="bottom-image">
    <img src="../assets/images/FacilityShowAll.png" alt="设备" />
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import * as echarts from 'echarts';

// 数据定义
const runningTime = ref(0.00);
const stopTime = ref(0.00);
const productionEfficiency = ref(0.0);
const cabinetTemperature = ref(0.0);
const workshopTemperature = ref(0.0);
const plannedOutput = ref(0);
const actualOutput = ref(0);
const completionEfficiency = ref(0.0);
const printGroup = ref({
  temperature: 0.0,
  windSpeed: 0.0
});
const bridgeArea1 = ref({
  temperature: 0.0,
  windSpeed: 0.0
});
const bridgeArea2 = ref({
  temperature: 0.0,
  windSpeed: 0.0
});
const bridgeArea3 = ref({
  temperature: 0.0,
  windSpeed: 0.0
});
const concentration = ref(0.0);
const unwindPullTension = ref(0);
const unwindTension = ref(0);
const unwindTurretPosition = ref(0);
const unwindDiameter = ref(0);
const windPullTension = ref(0);
const windTension = ref(0);
const windTurretPosition = ref(0);
const windDiameter = ref(0);

// 印刷速度仪表盘
const dashboard = ref(null);
const printingSpeed = ref(50); // 示例印刷速度

onMounted(() => {
  const myChart = echarts.init(dashboard.value);
  const option = {
    series: [
      {
        type: 'gauge',
        axisLine: {
          lineStyle: {
            width: 30
          }
        },
        pointer: {
          itemStyle: {
            color: 'auto'
          }
        },
        axisTick: {
          distance: -30,
          length: 8,
          lineStyle: {
            color: '#fff',
            width: 2
          }
        },
        splitLine: {
          distance: -30,
          length: 30,
          lineStyle: {
            color: '#fff',
            width: 4
          }
        },
        axisLabel: {
          color: 'auto',
          distance: 40,
          fontSize: 14
        },
        detail: {
          formatter: '{value} m/min',
          color: 'auto',
          fontSize: 30
        },
        data: [
          {
            value: printingSpeed.value,
            name: '印刷速度'
          }
        ]
      }
    ]
  };
  myChart.setOption(option);
});
</script>

<style scoped>
/* 全局样式 */
body {
  margin: 0;
  padding: 0;
  font-family: Arial, sans-serif;
}

/* 仪表盘和信息行容器 */
.dashboard-info-row {
  display: flex;
  align-items: center;
  padding: 20px;
}

.info-group {
  width: 60%;
}

/* 信息行样式 */
.info-row {
  display: flex;
  justify-content: space-around;
  background-color: #f0f0f0;
  padding: 10px;
}

.info-item {
  display: flex;
  align-items: center;
}

.info-item span {
  margin-right: 5px;
}

/* 中间区域模块样式 */
.middle-modules {
  display: flex;
  justify-content: space-around;
  padding: 20px;
}

/* 中间往下区域模块样式 */
.lower-modules {
  display: flex;
  justify-content: space-around;
  padding: 20px;
}

/* 模块样式 */
.module {
  border: 1px solid #ccc;
  border-radius: 5px;
  padding: 10px;
  width: 18%;
}

.module h3 {
  margin-top: 0;
}

.param-item {
  margin-bottom: 5px;
}

/* 数字显示区域样式 */
.number-display {
  background-color: #e0e0e0;
  padding: 3px 8px;
  border-radius: 4px;
  margin-left: 5px;
}

/* 底部图片样式 */
.bottom-image {
  text-align: center;
  padding: 20px;
}

.bottom-image img {
  max-width: 100%;
  height: auto;
}
</style>    