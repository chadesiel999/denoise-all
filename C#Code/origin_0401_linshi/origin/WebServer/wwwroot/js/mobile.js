//function floatingTouch() {
//    var flag = 0; //标记是拖曳还是点击
//    var oDiv = document.getElementById('monitor-touch');
//    var disX, moveX, L, T, starX, starY, starXEnd, starYEnd;
//    oDiv.addEventListener('touchstart', function (e) {
//        flag = 0;
//        e.preventDefault();//阻止触摸时页面的滚动，缩放
//        disX = e.touches[0].clientX - this.offsetLeft;
//        disY = e.touches[0].clientY - this.offsetTop;
//        //手指按下时的坐标
//        starX = e.touches[0].clientX;
//        starY = e.touches[0].clientY;
//        //console.log(disX);
//    });
//    oDiv.addEventListener('touchmove', function (e) {
//        flag = 1;
//        L = e.touches[0].clientX - disX;
//        T = e.touches[0].clientY - disY;
//        //移动时 当前位置与起始位置之间的差值
//        starXEnd = e.touches[0].clientX - starX;
//        starYEnd = e.touches[0].clientY - starY;
//        //console.log(L);
//        if (L < 0) {//限制拖拽的X范围，不能拖出屏幕
//            L = 0;
//        } else if (L > document.documentElement.clientWidth - this.offsetWidth) {
//            L = document.documentElement.clientWidth - this.offsetWidth;
//        }
//        if (T < 0) {//限制拖拽的Y范围，不能拖出屏幕
//            T = 0;
//        } else if (T > document.documentElement.clientHeight - this.offsetHeight) {
//            T = document.documentElement.clientHeight - this.offsetHeight;
//        }
//        moveX = L + 'px';
//        moveY = T + 'px';
//        //console.log(moveX);
//        this.style.left = moveX;
//        this.style.top = moveY;
//    });
//    oDiv.addEventListener('touchend', function (e) {
//        //alert(parseInt(moveX))
//        //判断滑动方向
//        if (flag === 0) {//点击
//            exitFullScreen();
//        }
//    });
//}

//window.onload = floatingTouch();


/**
 * 在页面添加一个拖拽按钮
 */

(function (global) {
    "use strict";

    var DragButtion = function () {
    }

    DragButtion.prototype = {
        //window对象
        win: window,
        //拖拽dom
        ele: null,
        //默认配置
        options: {
            edge: true, //是否吸附边缘，默认吸附
            url: null, //点击后要跳转的路径,默认不跳转
            extRoute: null,//标签Ext的路由，进行跳转的
            icon: null,//dom图标
            elemId: 'drag-buttion' //组件要加载的节点位置ID，默认加在body下
        },
        //系统变量集
        data: {
            distanceX: 0,
            distanceY: 0
        },

        /**
         * @method 初始化
         * @param { object } 由@method config() 提供的配置参数
         */
        init: function (opts, styleOpts) {
            var _this = this;
            var option = _this.config(opts, _this.options);//用户配置
            var _elem = document.getElementById(option.elemId) ? document.getElementById(option.elemId) : document.body;
            if (!_elem) {
                console.log("not find nodeId!!");
                return;
            }
            //初始拖拽按钮，加载到_elem内
            _this.initEle(_elem);
            //注册点击事件
            _this.ele.addEventListener('click', function () {
                _this.click();
            });
            //注册拖拽开始事件
            _this.ele.addEventListener('touchstart', function (event) {
                _this.touchstart(event);
            });
            //注册拖拽移动事件
            document.addEventListener(
                'touchmove',
                function (event) {
                    _this.touchmove(event);
                }, {// fix #3 #5
                passive: false
            });
            //注册拖拽完成事件
            document.addEventListener('touchend', function () {
                _this.touchend();
            });

        },
        /**
         * @method 配置
         * @param opts { object } 用户提供的参数，在没有提供参数的情况下使用默认参数 
         * @param options { object } 默认参数
         * @return options { object } 返回一个配置对象
         */
        config: function (opts, options) {
            //默认参数
            if (!opts)
                return options;
            for (var key in opts) {
                if (!!opts[key]) {
                    options[key] = opts[key];
                }
            }
            return options;
        },

        /**
         * @method 初始拖拽按钮，加载到_elem内
         * @param _elem { object } 指定挂载的节点
         * @return options { object } 返回一个拖拽按钮
         */
        initEle: function (_elem) {
            var _this = this;
            //创建一个div
            var ele = document.createElement('div');
            ele.id = "drag_buttion_id";
            ele.className = "drag-buttion-div";

            //样式
            ele.style.position = "fixed";
            ele.style.lineHeight = "4.75rem";
            ele.style.width = "4.75rem";
            ele.style.height = "4.75rem";
            ele.style.padding = "0.5rem";
            ele.style.textAlign = "center";
            ele.style.borderRadius = "99px";
            ele.style.color = "#fff";
            ele.style.backgroundColor = "#f2efef"//"#00bc12";
            ele.style.backgroundClip = "padding-box";
            ele.style.textDecoration = "none";
            ele.style.top = "13em";
            ele.style.right = "0px";
            ele.style.zIndex = "1";

            if (_this.options.icon) {
                ele.style.backgroundImage = "url(" + _this.options.icon + ")";
                ele.style.backgroundSize = "100% 100%";
            }

            //动态插入到body中
            _elem.insertBefore(ele, _elem.lastChild);
            //赋值到全局变量
            _this.ele = ele;
            //初始化位置
            var strStoreDistance = '';
            // 居然有android机子不支持localStorage
            if (_this.ele.id && _this.win.localStorage && (strStoreDistance = localStorage['Inertia_' + _this.ele.id])) {
                var arrStoreDistance = strStoreDistance.split(',');
                _this.ele.distanceX = +arrStoreDistance[0];
                _this.ele.distanceY = +arrStoreDistance[1];
                _this.ele = _this.fnTranslate(_this.ele, _this.ele.distanceX, _this.ele.distanceY);
            }
            // 显示拖拽元素
            _this.ele.style.visibility = 'visible';
            // 如果元素在屏幕之外，位置使用初始值
            var initBound = _this.ele.getBoundingClientRect();
            if (initBound.left < -0.5 * initBound.width ||
                initBound.top < -0.5 * initBound.height ||
                initBound.right > _this.win.innerWidth + 0.5 * initBound.width ||
                initBound.bottom > _this.win.innerHeight + 0.5 * initBound.height
            ) {
                _this.ele.distanceX = 0;
                _this.ele.distanceY = 0;
                _this.ele = _this.fnTranslate(_this.ele, 0, 0);
            }
        },

        /**
         * easeOutBounce算法
         * t: current time（当前时间）；
         * b: beginning value（初始值）；
         * c: change in value（变化量）；
         * d: duration（持续时间）。
         */
        easeOutBounce: function (t, b, c, d) {
            if ((t /= d) < (1 / 2.75)) {
                return c * (7.5625 * t * t) + b;
            } else if (t < (2 / 2.75)) {
                return c * (7.5625 * (t -= (1.5 / 2.75)) * t + 0.75) + b;
            } else if (t < (2.5 / 2.75)) {
                return c * (7.5625 * (t -= (2.25 / 2.75)) * t + 0.9375) + b;
            } else {
                return c * (7.5625 * (t -= (2.625 / 2.75)) * t + 0.984375) + b;
            }
        },

        // 设置transform坐标等方法
        fnTranslate: function (_ele, x, y) {
            x = Math.round(1000 * x) / 1000;
            y = Math.round(1000 * y) / 1000;

            _ele.style.webkitTransform = 'translate(' + [x + 'px', y + 'px'].join(',') + ')';
            _ele.style.transform = 'translate3d(' + [x + 'px', y + 'px', 0].join(',') + ')';

            return _ele
        },

        /**
         * 拖拽按钮开始事件
         */
        touchstart: function (event) {
            var _this = this;
            var events = event.touches[0] || event;

            _this.data.posX = events.pageX;
            _this.data.posY = events.pageY;

            _this.data.touching = true;

            if (_this.ele.distanceX) {
                _this.data.distanceX = _this.ele.distanceX;
            }
            if (_this.ele.distanceY) {
                _this.data.distanceY = _this.ele.distanceY;
            }

            // 元素的位置数据
            _this.data.bound = _this.ele.getBoundingClientRect();

            _this.data.timerready = true;
        },

        /**
         * 拖拽按钮移动事件
         */
        touchmove: function (event) {
            var _this = this;
            if (_this.data.touching !== true) {
                return;
            }

            // 当移动开始的时候开始记录时间
            if (_this.data.timerready == true) {
                _this.data.timerstart = +new Date();
                _this.data.timerready = false;
            }

            event.preventDefault();

            var events = event.touches[0] || event;

            _this.data.nowX = events.pageX;
            _this.data.nowY = events.pageY;

            var distanceX = _this.data.nowX - _this.data.posX,
                distanceY = _this.data.nowY - _this.data.posY;

            // 此时元素的位置
            var absLeft = distanceX + _this.data.bound.left,
                absTop = distanceY + _this.data.bound.top,
                absRight = absLeft + _this.data.bound.width,
                absBottom = absTop + _this.data.bound.height;

            // 边缘检测
            if (absLeft < 0) {
                distanceX = distanceX - absLeft;
            }
            if (absTop < 0) {
                distanceY = distanceY - absTop;
            }
            if (absRight > _this.win.innerWidth) {
                distanceX = distanceX - (absRight - _this.win.innerWidth);
            }
            if (absBottom > _this.win.innerHeight) {
                distanceY = distanceY - (absBottom - _this.win.innerHeight);
            }

            // 元素位置跟随
            var x = _this.data.distanceX + distanceX,
                y = _this.data.distanceY + distanceY;
            _this.ele = _this.fnTranslate(_this.ele, x, y);

            // 缓存移动位置
            _this.ele.distanceX = x;
            _this.ele.distanceY = y;
        },

        /**
         * 拖拽按钮移动完成事件
         */
        touchend: function () {
            var _this = this;
            if (_this.data.touching === false) {
                // fix iOS fixed bug
                return;
            }
            _this.data.touching = false;

            // 计算速度
            _this.data.timerend = +new Date();

            if (!_this.data.nowX || !_this.data.nowY) {
                return;
            }

            // 移动的水平和垂直距离
            var distanceX = _this.data.nowX - _this.data.posX,
                distanceY = _this.data.nowY - _this.data.posY;

            if (Math.abs(distanceX) < 5 && Math.abs(distanceY) < 5) {
                return;
            }

            // 距离和时间
            var distance = Math.sqrt(distanceX * distanceX + distanceY * distanceY), time = _this.data.timerend - _this.data.timerstart;

            // 速度，每一个自然刷新此时移动的距离
            var speed = distance / time * 16.666;

            // 经测试，2~60多px不等
            // 设置衰减速率
            // 数值越小，衰减越快
            var rate = Math.min(10, speed);

            // 开始惯性缓动
            _this.data.inertiaing = true;

            // 反弹的参数
            var reverseX = 1, reverseY = 1;

            // 速度计算法
            var step = function () {
                if (_this.data.touching == true) {
                    _this.data.inertiaing = false;
                    return;
                }
                speed = speed - speed / rate;

                // 根据运动角度，分配给x, y方向
                var moveX = reverseX * speed * distanceX / distance, moveY = reverseY * speed * distanceY / distance;

                // 此时元素的各个数值
                var bound = _this.ele.getBoundingClientRect();

                if (moveX < 0 && bound.left + moveX < 0) {
                    moveX = 0 - bound.left;
                    // 碰触边缘方向反转
                    reverseX = reverseX * -1;
                } else if (moveX > 0 && bound.right + moveX > _this.win.innerWidth) {
                    moveX = _this.win.innerWidth - bound.right;
                    reverseX = reverseX * -1;
                }

                if (moveY < 0 && bound.top + moveY < 0) {
                    moveY = -1 * bound.top;
                    reverseY = -1 * reverseY;
                } else if (moveY > 0 && bound.bottom + moveY > _this.win.innerHeight) {
                    moveY = _this.win.innerHeight - bound.bottom;
                    reverseY = -1 * reverseY;
                }

                var x = _this.ele.distanceX + moveX, y = _this.ele.distanceY + moveY;
                // 位置变化
                _this.ele = _this.fnTranslate(_this.ele, x, y);

                _this.ele.distanceX = x;
                _this.ele.distanceY = y;

                if (speed < 0.1) {
                    speed = 0;
                    if (_this.options.edge == false) {
                        data.inertiaing = false;

                        if (_this.win.localStorage) {
                            localStorage['Inertia_' + _this.ele.id] = [x, y].join();
                        }
                    } else {
                        // 边缘吸附
                        edge();
                    }
                } else {
                    requestAnimationFrame(step);
                }
            };

            var edge = function () {
                // 时间
                var start = 0, during = 25;
                // 初始值和变化量
                var init = _this.ele.distanceX, y = _this.ele.distanceY, change = 0;
                // 判断元素现在在哪个半区
                var bound = _this.ele.getBoundingClientRect();
                if (bound.left + bound.width / 2 < _this.win.innerWidth / 2) {
                    change = -1 * bound.left;
                } else {
                    change = _this.win.innerWidth - bound.right;
                }

                var run = function () {
                    // 如果用户触摸元素，停止继续动画
                    if (_this.data.touching == true) {
                        _this.data.inertiaing = false;
                        return;
                    }

                    start++;
                    var x = _this.easeOutBounce(start, init, change, during);
                    _this.ele = _this.fnTranslate(_this.ele, x, y);

                    if (start < during) {
                        requestAnimationFrame(run);
                    } else {
                        _this.ele.distanceX = x;
                        _this.ele.distanceY = y;
                        _this.data.inertiaing = false;
                        if (_this.win.localStorage) {
                            localStorage['Inertia_' + _this.ele.id] = [x, y].join();
                        }
                    }
                };
                run();
            };

            step();
        },
        /**
         * 拖拽按钮点击事件
         */
        click: function () {
            var _this = this;
            //跳转到配置的url中
            if (_this.options.url) {
                _this.win.location.href = _this.options.url;
            }
            if (_this.options.extRoute) {
                EU.setActiveItem(_this.options.extRoute);
            }
        }
    }

    global.DragButtion = DragButtion;//注册到全局中， 届时可以直接new DragButtion() 实例化对象
}(this))