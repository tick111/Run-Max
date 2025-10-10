import React from 'react'
import $ from 'jquery'

class ApiCall extends React.Component {
   constructor(props){
    super(props);
    this.state ={userName:'',lastUrl:''};
   }

   componentDidMount(){
    this.serverRequest = $.get(this.props.source)
        .done(function(result){
            console.log('API Response:', result); // Debug log
            
            // Check if result is an array with data
            if (result && result.length > 0) {
                var data = result[0];
                console.log('First gist data:', data); // Debug log
                
                // Extract username from the source URL
                var userName = 'Unknown User';
                if (this.props.source && this.props.source.includes('/users/')) {
                    var urlParts = this.props.source.split('/users/');
                    if (urlParts.length > 1) {
                        userName = urlParts[1].split('/')[0];
                    }
                }
                
                // Use the actual properties from the GitHub gists API
                this.setState({
                    userName: userName,
                    lastUrl: data.html_url || '#'
                });
            } else {
                console.error('No gists found in API response');
                this.setState({
                    userName: 'No gists found',
                    lastUrl: '#'
                });
            }
        }.bind(this))
        .fail(function(xhr, status, error) {
            console.error('API call failed:', error);
            this.setState({
                userName: 'Error loading user',
                lastUrl: '#'
            });
        }.bind(this));
   }

   componentWillUnmount(){
    this.serverRequest.abort();
   }

   render(){
    return(
        <div>
          <p><strong>{this.state.userName}</strong>'s latest gist:</p>
          <a href={this.state.lastUrl} target="_blank" rel="noopener noreferrer">
            {this.state.lastUrl}
          </a>
        </div>
    )
}
}

export default ApiCall;
   

