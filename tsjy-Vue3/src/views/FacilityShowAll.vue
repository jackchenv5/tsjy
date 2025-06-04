<template>
  <div class="container">
    <div class="header">
      <div class="header-item">
        <div style="height: 70%; width: 70%">
          <el-progress type="dashboard" :percentage="80" :stroke-width="20">
            <template #default="{ percentage }">
              <p class="percentage-value">{{ percentage }} m/min</p>
              <p class="percentage-label">印刷速度</p>
            </template>
          </el-progress>
        </div>
      </div>
      <div class="header-item">
        <div style="height: 70%; width: 70%">
          <el-progress type="dashboard" :percentage="80" :stroke-width="20">
            <template #default="{ percentage }">
              <p class="percentage-value">{{ percentage }} kw</p>
              <p class="percentage-label">加热功率</p>
            </template>
          </el-progress>
        </div>
      </div>
      <div class="header-item">
        <div style="height: 70%; width: 70%">
          <el-progress type="dashboard" :percentage="80" :stroke-width="20">
            <template #default="{ percentage }">
              <p class="percentage-value">{{ percentage }} kw</p>
              <p class="percentage-label">设备功率</p>
            </template>
          </el-progress>
        </div>
      </div>
      <div class="header-item">
        <div style="height: 70%; width: 70%">
          <el-progress type="dashboard" :percentage="80" :stroke-width="20">
            <template #default="{ percentage }">
              <p class="percentage-value">{{ percentage }} kw</p>
              <p class="percentage-label">排风功率</p>
            </template>
          </el-progress>
        </div>
      </div>
      <div class="header-item">
        <div style="height: 70%; width: 70%">
          <el-progress type="dashboard" :percentage="80" :stroke-width="20">
            <template #default="{ percentage }">
              <p class="percentage-value">{{ percentage }} /A</p>
              <p class="percentage-label">用电量</p>
            </template>
          </el-progress>
        </div>
      </div>
      <div class="last-header-item">
        <div class="header-info-item">
          <p>柜内温度 0 C</p>
        </div>
        <div class="header-info-item">
          <p>车间温度 0 C</p>
        </div>
        <div class="header-info-item">
          <p>柜内湿度 0 C</p>
        </div>
        <div class="header-info-item">
          <p>车间湿度 0 C</p>
        </div>
      </div>
    </div>
    <div class="mid">
      <el-card class="mid-card">
        <template #header>
          <div class="card-header">
            <span>印刷组</span>
          </div>
        </template>
        <p><span>温度：</span>20℃<span></span></p>
        <p><span>进风风速：</span>20℃<span></span></p>
        <p><span>回风风速：</span>20℃<span></span></p>
      </el-card>
      <el-card class="mid-card">
        <template #header>
          <div class="card-header">
            <span>天桥一区</span>
          </div>
        </template>
        <p><span>温度：</span>20℃<span></span></p>
        <p><span>风速：</span>20℃<span></span></p>
      </el-card>
      <el-card class="mid-card">
        <template #header>
          <div class="card-header">
            <span>天桥二区</span>
          </div>
        </template>
        <p><span>温度：</span>20℃<span></span></p>
        <p><span>风速：</span>20℃<span></span></p>
      </el-card>
      <el-card class="mid-card">
        <template #header>
          <div class="card-header">
            <span>天桥三区</span>
          </div>
        </template>
        <p><span>温度：</span>20℃<span></span></p>
        <p><span>风速：</span>20℃<span></span></p>
      </el-card>
    </div>
    <div class="footer">
      <div class="footer-item">
        <p class="footer-info">放牵张力：10</p>
      </div>
      <div class="footer-item">
        <p class="footer-info">收牵张力：10</p>
      </div>
      <div class="footer-item">
        <p class="footer-info" style="margin-top: 50px">放料张力：10</p>
      </div>
      <div class="footer-item">
        <p class="footer-info" style="margin-top: 50px">收料张力：10</p>
      </div>
    </div>
  </div>
</template>

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
  height: 20%;
  display: flex;
}

.last-header-item {
  flex: 1;
  display: flex;
  justify-content: space-around;
  align-items: center;
  flex-wrap: wrap;
}

.header-item {
  min-width: 200px;
  border: 1px solid white;
  display: flex;
  justify-content: center; // 水平居中
  align-items: center; // 垂直居中
}

.header-info-item {
  width: 48%;
  height: 48%;
  background-color: #eeeeef;
  display: flex;
  justify-content: center; // 水平居中
  align-items: center; // 垂直居中
}

.mid {
  display: flex;
  gap: 1vw;
  height: 30%;
}
.mid-card {
  min-width: 19%;
  flex: 1;
}
.last-mid-card {
  min-width: 0;
  flex: 1;
}

.footer {
  min-height: 0px;
  background-image: url('/device.jpg');
  background-repeat: no-repeat;
  /* 防止图片重复 */
  background-position: center;
  /* 图片居中 */
  background-size: cover;
  /* 图片覆盖整个元素 */
  flex: 1;
  display: flex;
  justify-content: center;
}
.footer-item {
  min-width: 200px;
  margin-top: 2%;
}
.footer-info {
  margin: 10px 20px;
  padding: 8px;
  font-size: 2rem;
  background-color: white;
  border-radius: 7px;
  width: 80%;
}
</style>
