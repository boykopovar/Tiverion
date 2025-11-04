(function () {
    const canvas = document.getElementById('statsChart');
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    const noDataEl = document.getElementById('no-data');
    let chartInstance = null;

    function hasData(cfg) {
        if (!cfg) return false;
        if (Array.isArray(cfg.labels) && cfg.labels.length > 0) return true;
        if (Array.isArray(cfg.datasets) && cfg.datasets.some(ds => Array.isArray(ds.data) && ds.data.length > 0)) return true;
        return false;
    }

    function toChartJsConfig(cfg) {
        const c = {
            type: cfg.type || 'line',
            data: {
                labels: cfg.labels || [],
                datasets: (cfg.datasets || []).map(ds => {
                    const base = {
                        label: ds.label || '',
                        data: ds.data || [],
                        fill: ds.fill || false,
                        tension: typeof ds.tension === 'number' ? ds.tension : 0.3,
                    };
                    if (ds.pointRadius !== undefined && ds.pointRadius !== null) base.pointRadius = ds.pointRadius;
                    return base;
                })
            },
            options: cfg.options || { responsive: true, maintainAspectRatio: false }
        };
        return c;
    }

    function destroyChart() {
        if (chartInstance) {
            chartInstance.destroy();
            chartInstance = null;
        }
    }

    function renderFromConfig(cfg) {
        destroyChart();
        if (!hasData(cfg)) {
            if (noDataEl) noDataEl.style.display = 'block';
            return;
        } else {
            if (noDataEl) noDataEl.style.display = 'none';
        }
        const chartJsCfg = toChartJsConfig(cfg);
        chartInstance = new Chart(ctx, chartJsCfg);
    }

    function updateChartConfig(newConfig) {
        window.__chartConfig = newConfig;
        renderFromConfig(newConfig);
    }

    function getCurrentConfig() {
        return window.__chartConfig;
    }
    
    window.StatsChart = {
        render: () => renderFromConfig(window.__chartConfig),
        update: updateChartConfig,
        getConfig: getCurrentConfig
    };
    
    try {
        renderFromConfig(window.__chartConfig);
    } catch (e) {
        console.error('StatsChart render error', e);
    }

})();
