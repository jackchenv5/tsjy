<template>
  <div class="container">
    <div class="header">
      <div class="header-card-container">
        <div
          style="height: 100%; display: flex; margin-left: 20px; flex-grow: 1"
        >
          <div class="progress-container">
            <el-progress
              width="90"
              type="dashboard"
              :percentage="80"
              stroke-width="16"
              class="custom-progress"
            >
              <template #default="{ percentage }">
                <p class="percentage-value">{{ percentage }}</p>
                <span class="percentage-label">OEE</span>
              </template>
            </el-progress>
          </div>

          <div
            style="
              margin-left: 20px;
              display: flex;
              flex-direction: column;
              height: 100%;
              justify-content: center;
              gap: 10px;
              align-items: center;
            "
          >
            <div style="font-size: 1.6rem; font-weight: bold">
              当前设备运行正常
            </div>
            <div style="font-size: 1.2rem; color: #999">已无故障运行 30 天</div>
          </div>
        </div>
        <div style="width: 32%; display: flex; gap: 5vw">
          <div class="text-container">
            <span class="text-label"> 运行时长 </span
            ><span class="text-content">12/20</span>
          </div>
          <div class="text-container">
            <span class="text-label"> 产量 </span
            ><span class="text-content">800</span>
          </div>
          <div class="text-container">
            <span class="text-label"> 良品率 </span
            ><span class="text-content">80%</span>
          </div>
        </div>
      </div>
    </div>
    <div class="mid">
      <el-tabs v-model="activeName" class="demo-tabs" @tab-click="handleClick">
        <el-tab-pane label="时长" name="first"></el-tab-pane>
        <el-tab-pane label="产量" name="second"></el-tab-pane>
        <el-tab-pane label="良品率" name="third"></el-tab-pane>
      </el-tabs>
      <BarChart />
    </div>
    <div class="footer">
      <el-card class="footer-card" header="运行时长">
        <PieChartOne />
      </el-card>
      <el-card class="footer-card" header="产量">
        <PieChartTwo />
      </el-card>
      <el-card class="footer-card" header="良品率">
        <PieChartThree />
      </el-card>
    </div>
    <!-- 结构不变 -->
  </div>
</template>
<script setup>
import BarChart from './oeeChart/BarChart.vue'
import PieChartOne from './oeeChart/PieChart1.vue'
import PieChartTwo from './oeeChart/PieChart2.vue'
import PieChartThree from './oeeChart/PieChart3.vue'
</script>
<style scoped lang="scss">
* {
  box-sizing: border-box;
}

.container {
  display: flex;
  flex-direction: column;
  height: 100%;
  min-height: 800px;
  background-color: #efefef;
}

.header {
  border: 1px solid wihte;
  height: 15%;
  background-color: #fff;
}

.header-card-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 100%;
}
.progress-container {
  display: flex;
  justify-content: center;
  align-items: center;
}
.text-container {
  display: flex;
  flex-direction: column;
  gap: 5px;
}
.text-label {
  color: #999;
}
.text-content {
  font-size: 1.6rem;
  font-weight: bold;
}
.mid {
  margin-top: 10px;
  background-color: white;
}
.footer {
  margin-top: 10px;
  flex-grow: 1;
  background-color: #eee;
  display: flex;
  justify-content: space-between;
  align-items: stretch; /* 添加此属性，使卡片高度相同且撑满 */
}
.footer-card {
  width: 33%;
  height: 100%;
}
</style>
