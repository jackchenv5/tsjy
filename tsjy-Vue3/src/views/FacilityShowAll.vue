<template>
    <div class="container">
        <div class="header">
            <div class="header-item">
                <div style="height: 70%;width: 70%;">
                    <el-progress type="dashboard" :percentage="80" :stroke-width="20">
                        <template #default="{ percentage }">
                            <p class="percentage-value">{{ percentage }} m/min</p>
                            <p class="percentage-label">印刷速度</p>
                        </template>
                    </el-progress>
                </div>
            </div>
            <div class="header-item">
                <div style="height: 70%;width: 70%;">
                    <el-progress type="dashboard" :percentage="80" :stroke-width="20">
                        <template #default="{ percentage }">
                            <p class="percentage-value">{{ percentage }} kw</p>
                            <p class="percentage-label">加热功率</p>
                        </template>
                    </el-progress>
                </div>
            </div>
            <div class="header-item">
                <div style="height: 70%;width: 70%;">
                    <el-progress type="dashboard" :percentage="80" :stroke-width="20">
                        <template #default="{ percentage }">
                            <p class="percentage-value">{{ percentage }} kw</p>
                            <p class="percentage-label">设备功率</p>
                        </template>
                    </el-progress>
                </div>
            </div>
            <div class="header-item">

                <div style="height: 70%;width: 70%;">
                    <el-progress type="dashboard" :percentage="80" :stroke-width="20">
                        <template #default="{ percentage }">
                            <p class="percentage-value">{{ percentage }} kw</p>
                            <p class="percentage-label">排风功率</p>
                        </template>
                    </el-progress>
                </div>
            </div>
            <div class="header-item">

                <div style="height: 70%;width: 70%;">
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
        <div class="mid">mid</div>
        <div class="footer">footer</div>
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
    border: 1px solid black;
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
    background-color: #efefef;
    border: 1px solid white;
    display: flex;
    justify-content: center; // 水平居中
    align-items: center; // 垂直居中
}



.mid {
    border: 1px solid yellow;
    height: 30%;
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
}
</style>
