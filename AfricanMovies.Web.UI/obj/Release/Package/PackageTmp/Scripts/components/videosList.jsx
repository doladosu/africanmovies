/**
 * @jsx React.DOM
 */

 var VideosOrder = React.createClass({

 	getInitialState: function(){

 		var currentField = this.props.orders.filter(function(field){
 			return field.field === this.props.currentSettings.PropertyName;
 		}, this);

 		return {
 			current: {
 				field: currentField[0],
 				direction: this.props.currentSettings.Direction
 			}
 		};
 	},

 	sortOrderChanged: function(event){

 		var target = event.target;

 		event.preventDefault();

 		if(event.target.nodeName === "SPAN")
 		{
 			target = event.target.parentElement;
 		}

 		var data = $(target).data(),
 			currentField = this.props.orders.filter(function(field){
 				return field.field === data.field;
 			}, this);

 		this.setState({ current: {
 			field: currentField[0],
 			direction: data.orderDirection
 		}});

 		$('#'+this.props.componentName)
 		.trigger("changeOrder", {
 				PropertyName: data.field,
 				Direction: data.orderDirection
		});
 	},

 	render: function(){
 		
 		var icon = (this.state.current.direction === this.props.directions.ascending) 
 						? 'glyphicon glyphicon-sort-by-attributes'
 						: 'glyphicon glyphicon-sort-by-attributes-alt',
 			options = this.props.orders.map(function(field){

 				var active = field === this.state.current.field;

	 			return (<div>
			                <li className="divider"></li>
				            <li className={active && this.state.current.direction == this.props.directions.ascending ? 'active' : '' }>
				            	<a href="#" onClick={this.sortOrderChanged} data-order-direction={this.props.directions.ascending} data-field={field.field}>{field.label} <span className="glyphicon glyphicon-sort-by-attributes"></span></a>
			            	</li>
				            <li className={active && this.state.current.direction == this.props.directions.descending ? 'active' : '' }>
				            	<a href="#" onClick={this.sortOrderChanged}  data-order-direction={this.props.directions.descending} data-field={field.field}>{field.label} <span className="glyphicon glyphicon-sort-by-attributes-alt"></span></a>
			            	</li>
		            	</div>);
 			}.bind(this));

 		return (
 			    <div id={this.props.componentName} className="pull-right  col-sm-4 col-md-4 col-lg-4 text-right order">
			        Kolejność: 
			        <div className="btn-group">
			            <button type="button" className="btn btn-primary btn-xs dropdown-toggle" data-toggle="dropdown">
			                {this.state.current.field.label} <span className={icon}></span> <span className="caret"></span>
			            </button>
			            <ul className="dropdown-menu" role="menu">
			                {options}
			            </ul>
			        </div>
			    </div>
 			);
 	}

 });

 var VideosList = React.createClass({

 	render: function(){

 		var self = this,
 			footerStyle = {
 			'margin-left': '10px'
 			},
 			lastElement = {
 				'margin-left': '10px',
 				'margin-right': '5px'
 			},
 			shortenCaption = function(caption){

 				if(caption && caption.length > self.props.captionLength)
 				{ 	
 					var clipFrom = caption
 							.substring(0, self.props.captionLength)
 							.lastIndexOf(" ");

 					return caption.substring(0, clipFrom) + ' ...';
 				}

 				return caption;
 			},
 			prepareModal = function(video){
 				return (
 					<div className="modal fade bs-example-modal-lg" id={video.Id} tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
					    <div className="modal-dialog">
					        <div className="modal-content">
					            <div className="modal-header">
					                <button type="button" className="close" data-dismiss="modal" aria-hidden="true">×</button>
					                <h4 className="modal-title text-primary">{video.Title}</h4>
					            </div>
					            <div className="modal-body">
					                <p>{video.Description}</p>
					            </div>
					        </div>
					    </div>
					</div>
 					);
 			},
 			player = function(video){

 				if(video.Url.toLowerCase().indexOf("vimeo") !== -1){
 					var link = '//player.vimeo.com/video/'+video.Id;
 					return (<div className="embed-responsive embed-responsive-16by9">
 								<iframe className="embed-responsive-item" src={link} title="0" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>
							</div>);
 				}
 				else if(video.Url.toLowerCase().indexOf("youtube") !== -1){
 					var link = 'https://www.youtube.com/embed/'+video.Id;
 					return (<div className="embed-responsive embed-responsive-16by9">
 								<iframe className="embed-responsive-item" src={link} webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>
							</div>);
 				}

 				return <div></div>;

 			},
 			videos = this.props.videos.map(function(video){

 				var caption = shortenCaption(video.Description),
 					playerFrame = player(video),
 					modalId = '#'+video.Id,
 					modal = prepareModal(video),
 					tags = video.Tags.map(function(tag){

 						var containerStyle = {
 							'display': 'inline',
 							'float': 'left'
 						};

 						return <div style={containerStyle}><span className="label label-info">{tag}</span></div>;
 					});

 				return (
		 			<div className="video col-sm-4 col-lg-3 col-md-4">
		                <div className="thumbnail">
		                    {playerFrame}
		                    <div className="caption">
		                        <h4><a href={video.Url} title={video.Title}>{video.Title}</a></h4>
		                        <p data-toggle="modal" data-target={modalId} title={video.Description}>{caption}</p>
		                        <div className="clearfix">
		                        	{tags}                        
		                        </div>
		                    </div>
		                    <div>
		                        <p className="pull-right" style={lastElement}><span className="glyphicon glyphicon-thumbs-down"></span>{video.Dislikes}</p>
		                        <p className="pull-right" style={footerStyle}><span className="glyphicon glyphicon-thumbs-up"></span>{video.Likes} </p>
		                        <p className="pull-right" style={footerStyle}><span className="glyphicon glyphicon-eye-open"></span>{video.Views}</p>
		                        <p style={footerStyle}><a href={video.ChannelInfo.Link} title={video.ChannelInfo.Name}>{video.ChannelInfo.Name}</a></p>
		                    </div>
		                </div>
		                {modal}
		            </div>
 				);
 		});

 		return (
 			<div className="row">
 				{videos}
 			</div>
 			);
 	}

 });

var Pagination = React.createClass({


	goTo: function(event){

		var page = $(event.target).data().page;

		event.preventDefault();

		if(page === 0 || page === this.props.currentSettings.NumberOfPages + 1)
			return;

		$('#'+this.props.componentName)
		.trigger('changePage', { Page: page });
	},

	render: function() {

		var pages = [],
			firstPage = (function(self){
				var disabled = (self.props.currentSettings.Page === 1)
								? 'disabled'
								: '';

				return <li className={disabled}><a href="#" data-page={self.props.currentSettings.Page-1} onClick={self.goTo}>«</a></li>;
			})(this),
			lastPage = (function(self){
				var last = self.props.currentSettings.NumberOfPages,
					disabled = (self.props.currentSettings.Page === last)
								? 'disabled'
								: '';

				return <li className={disabled}><a href="#" data-page={self.props.currentSettings.Page+1} onClick={self.goTo}>»</a></li>;
			})(this);

		for (var current = this.props.currentSettings.Page,
				all = this.props.currentSettings.NumberOfPages,
				paginationSize = this.props.size,
				toSubtract = (current%paginationSize === 0)
					? 1
					: current%paginationSize,
				i = (current > paginationSize) 
					? current - toSubtract 
					: 1, 
				last = all,
				stop = (i + paginationSize > last) 
					? last + 1
					: i + paginationSize; 

					i < stop; i++) {
			
			var active = (i === current)
						? 'active'
						: '';

			pages.push(<li className={active}><a href="#" data-page={i} onClick={this.goTo}>{i}</a></li>);

		};

		pages.splice(0, 0, firstPage);
		pages.push(lastPage);

		return (
			<div id={this.props.componentName} className="text-center">				
	            <ul className="pagination">
	            	{pages}	                
	            </ul>
	        </div>
		);
	}

});