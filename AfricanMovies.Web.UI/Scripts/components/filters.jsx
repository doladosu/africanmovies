
var ChannelFilters = React.createClass({

	getInitialState: function(){
		return {
			initial: this.props.selected
		};
	},

	channelSelected: function(event){
		this.setState({ initial: event.target.value, current: event.target });		
		this.props.onChannelSelected(event.target.value);
	},

  	componentWillReceiveProps: function(nextProps){
  		if(this.props.resetState && !nextProps.resetState)
  		{
  			this.reset();
  		}
  	},

  	reset: function(){
  		this.state.current.checked = false;
  		this.setState({});
  	},

	render: function(){

		var channels = this.props.channels.map(function(channel){
				var checked = this.state.initial === channel;

			return (<div className="radio">
							<label>
								<input type="radio" name="channel" value={channel} checked={checked} onChange={this.channelSelected} />
								{channel}
							</label>
						</div>);
		}.bind(this));

		return (<div className="panel panel-success">
				<div className="panel-heading">
					<h3 className="panel-title">CHANNEL</h3>
				</div>
				<div className="panel-body">
					{channels}
				</div>
			</div>);
	}
});

var PublicationYearsFilters = React.createClass({

	getInitialState: function(){
		return {
			initial: this.props.selected
		};
	},

	yearSelected: function(event){
		var year = parseInt(event.target.value);

		this.setState({ initial: year, current: event.target });	
		this.props.onPublicationYearSelected(year);
	},

  	componentWillReceiveProps: function(nextProps){
  		if(this.props.resetState && !nextProps.resetState)
  		{
  			this.reset();
  		}
  	},

  	reset: function(){

  		this.state.current.checked = false;
  		this.setState({});

  	},

	render: function(){

		var publicationYears = this.props.years.map(function(year){

			var checked = this.state.initial === year;

			return (<div className="radio">
							<label>
								<input type="radio" name="publicationYear" value={year} checked={checked} onChange={this.yearSelected} />
								{year}
							</label>
						</div>);
		}.bind(this));

		return (<div className="panel panel-info">
				<div className="panel-heading">
					<h3 className="panel-title">YEAR OF PUBLICATION</h3>
				</div>
				<div className="panel-body">
					{publicationYears}
				</div>
			</div>);
	}
});

var TagsFilters = React.createClass({

	getInitialState: function() {
	    return { tags: this.props.selected || [] };
  	},

  	componentWillReceiveProps: function(nextProps){
  		if(this.props.resetState && !nextProps.resetState)
  		{
  			this.setState(this.getInitialState());
  		}
  	},

  	approved: function(event){
  		var tags = this.state.tags,
  			value = event.target.value,
  			selectedTag = this.props.tags.filter(function(tag){
 				return value && tag === value;
 			});

  		if(event.key === 'Enter' 
  			&& selectedTag.length === 1){
  			
  				tags.push(selectedTag[0]);
  				this.setState({ tags: tags });

  				this.props.onTagsChanged(tags);

  				event.target.value = null;
  			}
  	},

  	remove: function(event){
  		var tags = this.state.tags,
  			index = tags.indexOf(event.target.value);

  		tags.splice(index, 1);

  		this.setState({ tags: tags });

  		this.props.onTagsChanged(tags);
  	},

	render: function(){

		var self = this,
			tags = this.state.tags.map(function(tag){

			return (<span className="label label-primary pull-left" onClick={self.remove}>{tag} <span className="delete">&times;</span></span>);
		});

		return (<div className="panel panel-primary filters">
            <div className="panel-heading">
                <h3 className="panel-title">Tags</h3>
            </div>
            <div className="panel-body">
                <div className="form-group">
                    <div className="input-group">
                        <span className="input-group-addon"><span className="glyphicon glyphicon-tag"></span></span>
                        <input type="text" id="tags" className="form-control" placeholder="Wpisz tag" ref="tag" onKeyDown={this.approved} />
                    </div>
                </div>
                <div>
                    {tags}
                </div>
            </div></div>);
	}
});

var FiltersButtons = React.createClass({

	handle: function(event){
		
		var action = event.target.name;

		if(event.target.parentElement.tagName === "BUTTON")
			action = event.target.parentElement.name;

		this.props.onAction(action);
	},
	render: function() {
		return (
			<div className="btn-group btn-group-justified">
	            <div className="btn-group">
	                <button className="btn btn-primary btn-group-lg" name="filter" onClick={this.handle}><span className="glyphicon glyphicon-ok"></span> Apply Filters</button>
	            </div>
	            <div className="btn-group">
	                <button className="btn btn-danger btn-group-lg" name="clear" onClick={this.handle}><span className="glyphicon glyphicon-remove"></span> Remove Filters</button>
	            </div>
        	</div>
		);
	}
});

var VideosFilters = React.createClass({
	getInitialState: function() {
	    return {
	    	resetState: false,
	    	filters: this.props.data.Current
	    };
  	},
	channelSelected: function(channel){
		this.setState($.extend(this.state.filters, { ChannelName: channel }));
	},
	publicationYearSelected: function(year){
		this.setState($.extend(this.state.filters, { PublicationYear: year }));
	},
	tagsChanged: function(tags){
		this.setState($.extend(this.state.filters, { Tags: tags }));
	},

	actionHandler: function(action){

		var self = this,
			hasAnyFilters = function(){
				return JSON.stringify(self.state.filters) !== JSON.stringify({});
			};

		if(action === 'filter' && hasAnyFilters())
		{
			this.setState($.extend(this.state, { resetState: true }));

			$("#" + this.props.componentName).trigger("filter", this.state.filters);			
		}
		else if (action === 'clear')
		{
			this.setState({ resetState: false, filters: {}});

			$("#" + this.props.componentName).trigger("clear", { ChannelName: null, PublicationYear: null, Tags: [] });	
		}
	},
	render: function(){
		
		return (<div>
					<ChannelFilters selected={this.props.data.Current.ChannelName} onChannelSelected={this.channelSelected} resetState={this.state.resetState} channels={this.props.data.Channels} />
					<PublicationYearsFilters selected={this.props.data.Current.PublicationYear} onPublicationYearSelected={this.publicationYearSelected} resetState={this.state.resetState} years={this.props.data.PublicationYears} />
					<TagsFilters selected={this.props.data.Current.Tags} onTagsChanged={this.tagsChanged} resetState={this.state.resetState} tags={this.props.data.Tags} />
					<FiltersButtons onAction={this.actionHandler} />
				</div>);
	}
});