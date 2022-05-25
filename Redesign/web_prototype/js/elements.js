

class ProvenceCanvas extends HTMLElement{

    

    constructor(){
        super();
        this.RegisterEventListeners();
    }

    RegisterEventListeners(){}
}

class ProvenceNode extends HTMLElement{

    dragging = false;
    dragOffset = {x:0,y:0};
    resizeOffset = {x:0,y:0};
    resizePadding = 15;
    resizeLeft = false;
    resizeRight = false;
    resizeTop = false;
    resizeBottom = false;


    constructor(){
        super();
        this.registerEventListeners();
    }

    registerEventListeners(){
        $(this).mousedown((e)=>{
            var xPos =  e.clientX - $(this).offset().left;
            var yPos = e.clientY - $(this).offset().top;
            this.resizeOffset = {
                width: $(this).innerWidth(),
                height: $(this).innerHeight(),
                x: e.clientX,
                y: e.clientY
            };
            if(e.button === 0){
                if(xPos <= this.resizePadding) this.resizeLeft = true;
                if(yPos <= this.resizePadding) this.resizeTop = true;
                if($(this).width() - xPos + 7 <= this.resizePadding) this.resizeRight = true;
                if($(this).height() - yPos + 7 <= this.resizePadding) this.resizeBottom = true;
            }
            if(e.button === 1){
                this.dragging = true;
                this.dragOffset.x = xPos;
                this.dragOffset.y = yPos;
                $(this).css({cursor: "move"});
            }
        }).mouseup((e)=>{
            this.resetMouseEvents();
        }).mouseout((e)=>{
            this.resetMouseEvents();
        }).mousemove((e)=>{
            if(this.dragging){
                $(this).css({
                    left: e.clientX - this.dragOffset.x,
                    top: e.clientY - this.dragOffset.y
                });
            }
            if(this.resizeRight){
                $(this).width(this.resizeOffset.width + e.clientX - this.resizeOffset.x);
            }
            else if(this.resizeLeft){
                $(this).css({left: e.clientX - 5});
                $(this).width(this.resizeOffset.width + this.resizeOffset.x - e.clientX);
            }
            if(this.resizeBottom){
                $(this).height(this.resizeOffset.height + e.clientY - this.resizeOffset.y);
            }else if(this.resizeTop){
                $(this).css({top: e.clientY - 5});
                $(this).height(this.resizeOffset.height + this.resizeOffset.y - e.clientY);
            }
        });
    }

    resetMouseEvents(){
        this.dragging = false;
        $(this).css({cursor: "default"});
        this.resizeLeft = false;
        this.resizeRight = false;
        this.resizeTop = false;
        this.resizeBottom = false;
    }

}

customElements.define('provence-canvas', ProvenceCanvas);
customElements.define('provence-node',ProvenceNode);