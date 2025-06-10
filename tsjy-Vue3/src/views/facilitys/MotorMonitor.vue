<template>
  <div class="container">
    <div class="motor-label">
      <el-card style="height: 120px" header="电机名称：卷绕">
        电机型号：22554
      </el-card>
      <el-card style="height: 120px" header="电机名称：拖引">
        电机型号：22554
      </el-card>
    </div>
    <div class="motor-card">
      <el-card style="height: 150px" header="电机转速">
        <div style="display: flex; justify-content: space-between">
          <div>
            <div>额定:1200</div>
            <div>最大:2400</div>
          </div>
          <div>0.0 RPM</div>
        </div>
        <el-progress :text-inside="true" :stroke-width="16" :percentage="50" />
      </el-card>
      <el-card style="height: 150px" header="电机电流">
        <div style="display: flex; justify-content: space-between">
          <div>
            <div>额定:80</div>
            <div>最大:120</div>
          </div>
          <div>0.0 A</div>
        </div>
        <el-progress :text-inside="true" :stroke-width="16" :percentage="50" />
      </el-card>
      <el-card style="height: 150px" header="电机扭矩">
        <div style="display: flex; justify-content: space-between">
          <div>
            <div>额定:250</div>
            <div>最大:500</div>
          </div>
          <div>0.0 N.m</div>
        </div>
        <el-progress :text-inside="true" :stroke-width="16" :percentage="50" />
      </el-card>
      <el-card style="height: 150px" header="电机温度">
        <div style="display: flex; justify-content: space-between">
          <div>
            <div>额定:80</div>
            <div>最大:150</div>
          </div>
          <div>0.0 ℃</div>
        </div>
        <el-progress :text-inside="true" :stroke-width="16" :percentage="50" />
      </el-card>
      <el-card style="height: 150px" header="电机振幅">
        <div style="display: flex; justify-content: space-between">
          <div>
            <div>额定:80</div>
            <div>最大:150</div>
          </div>
          <div>0.0 mm</div>
        </div>
        <el-progress :text-inside="true" :stroke-width="16" :percentage="50" />
      </el-card>
    </div>
    <div class="montor-chart">
      <el-card>
        <RpmChart />
        <EleChart />
        <PowerChart />
        <TemChart />
        <AChart />
      </el-card>
    </div>
  </div>
</template>

<script setup>
import RpmChart from './charts/RpmChart.vue'
import EleChart from './charts/EleChart.vue'
import PowerChart from './charts/PowerChart.vue'
import TemChart from './charts/TemChart.vue'
import AChart from './charts/AChart.vue'
// 创建图表的容器引用
</script>

<style scoped lang="scss">
* {
  box-sizing: border-box;
}
.container {
  width: 100%;
  height: 100%;
  display: flex;
  gap: 10px;
}
.motor-label {
  width: 20%;
  height: 100%;
  background-color: #f5f5f5;
  display: flex;
  flex-direction: column;
  gap: 10px;
}
.motor-card {
  width: 20%;
  height: 100%;
  background-color: #f5f5f5;
  display: flex;
  flex-direction: column;
  gap: 10px;
}
.montor-chart {
  width: 60%;
  height: 100%;
  background-color: #f5f5f5;
  overflow: auto;
}
</style>
